using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using AskBotCore;
using AzureFunctions.Payloads;
using CloudLib;
using CloudLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SlackLib;
using SlackLib.Requests;

namespace AzureFunctions
{
    public class AskBotHandler
    {
        private readonly ILogger<AskBotHandler> _logger;
        private readonly IStorage _storage;
        private readonly ISlackClient _slackClient;
        private readonly IAskBotControl _control;

        public AskBotHandler(ILogger<AskBotHandler> logger, IStorage storage, ISlackClient slackClient, IAskBotControl control)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _slackClient = slackClient ?? throw new ArgumentNullException(nameof(slackClient));
            _control = control ?? throw new ArgumentNullException(nameof(control));
        }

        [FunctionName(nameof(AskBotHook))]
        public async Task<IActionResult> AskBotHook([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req)
        {
            _logger.LogDebug("AskBot hook launched");
            var contentString = await req.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(contentString))
            {
                throw new ArgumentException("Content of HttpRequest is empty.", nameof(req));
            }

            _logger.LogTrace("Parsing raw: {content}", contentString);
            var escaped = HttpUtility.ParseQueryString(contentString);
            var payloadString = escaped["payload"];
            if (string.IsNullOrWhiteSpace(payloadString))
            {
                throw new ArgumentException("No payload element found in content.", nameof(req));
            }

            _logger.LogDebug("Deserializing payload: {payload}", payloadString);
            var json = JsonSerializer.Deserialize<JsonElement>(payloadString);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            switch (json.GetProperty("type").GetString())
            {
                case "block_actions":
                    var blockAction = JsonSerializer.Deserialize<BlockAction>(payloadString, options);
                    await HandleBlockAction(blockAction);
                    return new OkResult();
                case "shortcut":
                    var shortcut = JsonSerializer.Deserialize<Shortcut>(payloadString, options);
                    await HandleShortcut(shortcut);
                    return new OkResult();
                case "view_submission":
                    var viewSubmission = JsonSerializer.Deserialize<ViewSubmission>(payloadString, options);
                    return await HandleViewSubmission(viewSubmission, Guid.NewGuid().ToString(), DateTime.UtcNow);
                default:
                    throw new NotImplementedException($"Unknown payload type {json.GetProperty("type").GetString()}.");
            }
        }

        private async Task HandleBlockAction(BlockAction blockAction)
        {
            if (blockAction.Actions.Count() != 1) throw new ArgumentException("The block action list did not have 1 element in", nameof(blockAction));

            var actionToHandle = blockAction.Actions.First();
            switch (actionToHandle.ActionId)
            {
                case "add_option":
                case "delete_option":
                    _logger.LogInformation("Adding/deleting option to questionnaire.");
                    var mainPayload = PayloadUtility.GetCreateQuestionnaireMainPayload(int.Parse(actionToHandle.Value));
                    var viewPayload = blockAction.GetAddOptionToQuestionnairePayload(mainPayload);

                    _logger.LogDebug("Updating slack model with new available options.");
                    await _slackClient.UpdateModelView(viewPayload);
                    break;
                case "open_questionnaire":
                    _logger.LogInformation("Questionnaire open request received from {channel} by {answerer}", blockAction.Channel.Name, blockAction.User.Username);
                    var questionnaireId = actionToHandle.Value;
                    var questionnaire = await _storage.GetQuestionnaire(questionnaireId);

                    ViewsOpenRequest questionnairePayload;
                    if (questionnaire is null)
                    {
                        _logger.LogDebug("Did not found questionnaire: {questionnaireId}.", questionnaireId);
                        questionnairePayload = blockAction.GetRemovedQuestionnaireViewPayload();
                    }
                    else
                    {
                        var previousAnswers = await _storage.GetAnswers(questionnaireId, blockAction.User.Username);
                        var previousAnswer = previousAnswers.FirstOrDefault();
                        questionnairePayload = blockAction.GetOpenQuestionnaireViewPayload(questionnaire, previousAnswer?.Answer);
                    }

                    _logger.LogInformation("Opening slack model to answer the questionnaire.");
                    await _slackClient.OpenModelView(questionnairePayload);
                    break;
                case "get_answers":
                    _logger.LogInformation("Sending answers to questionnaire: {questionnaireId}", actionToHandle.Value);
                    await _control.GetQuestionnaireResult(actionToHandle.Value).ConfigureAwait(false);
                    break;
                default:
                    throw new NotImplementedException($"Unknown blockAction callback id: {actionToHandle.ActionId}.");
            }
        }

        private async Task HandleShortcut(Shortcut shortcut)
        {
            _logger.LogInformation("Shortcut request received from {user} with callback ID: {callback}", shortcut.User.Username, shortcut.CallbackId);

            ViewsOpenRequest payload;
            switch (shortcut.CallbackId)
            {
                case "create_questionnaire":
                    payload = shortcut.GetOpenCreateQuestionnairesPayload(PayloadUtility.GetCreateQuestionnaireMainPayload());

                    _logger.LogInformation("Opening slack model to create questionnaire.");
                    break;
                case "delete_a_questionnaire":
                    _logger.LogInformation("Fetching questionnaires.");
                    var questionnaires = await _storage.GetQuestionnaires().ConfigureAwait(false);
                    _logger.LogDebug("Found {results} questionnaires.", questionnaires.Count());
                    if (questionnaires is null || questionnaires.Count() == 0)
                    {
                        payload = shortcut.GetNoQuestionnairesAvailablePayload();
                    }
                    else
                    {
                        payload = shortcut.GetOpenListOfQuestionnairesPayload(questionnaires, shortcut.CallbackId);
                    }

                    _logger.LogInformation("Opening slack model to list the questionnaires available.");
                    break;
                case "delete_questionnaires":
                    _logger.LogInformation("Send cornfirmation view.");
                    payload = shortcut.GetConfirmDeleteAllPayload();
                    break;
                default:
                    throw new NotImplementedException($"Unknown shortcut callback id: {shortcut.CallbackId}.");
            }
            await _slackClient.OpenModelView(payload);
        }

        private async Task<IActionResult> HandleViewSubmission(ViewSubmission viewSubmission, string guid, DateTime dateTime)
        {
            _logger.LogInformation("View submission received.");

            switch (viewSubmission.View.CallbackId)
            {
                case "create_questionnaire":
                    _logger.LogInformation("Creating and posting questionnaire received from {user}.", viewSubmission.User.Username);

                    var channel = viewSubmission.View.State.Values["ChannelBlock"]["channel"].Value;
                    if (string.IsNullOrWhiteSpace(channel))
                    {
                        _logger.LogCritical("View submission channel is empty.");
                        throw new ArgumentException("View submission channel is empty", nameof(viewSubmission));
                    }
                    var question = viewSubmission.View.State.Values["TitleBlock"]["title"].Value;
                    if (string.IsNullOrWhiteSpace(question))
                    {
                        _logger.LogCritical("View submission question is empty.");
                        throw new ArgumentException("View submission question is empty", nameof(viewSubmission));
                    }

                    var answerOptionDictionaries = viewSubmission.View.State.Values.Where(d => d.Key.Contains("Answer")).Select(kvp => kvp.Value);
                    var answerOptions = answerOptionDictionaries.Select(d => d.First().Value.Value).ToArray();

                    if (answerOptions.Count() == 0)
                    {
                        _logger.LogCritical("View submission answer options are empty.");
                        throw new ArgumentException("View submission answer options are empty.", nameof(viewSubmission));
                    }

                    var questionnaire = new QuestionnaireEntity(guid, channel)
                    {
                        Question = question,
                        AnswerOptions = answerOptions,
                        Created = dateTime
                    };

                    _logger.LogDebug("Questionnaire ready to be created.");
                    await _control.CreateQuestionnaire(questionnaire).ConfigureAwait(false);
                    return new OkResult();
                case "open_questionnaire":
                    _logger.LogInformation("Answer received from {answerer}.", viewSubmission.User.Username);
                    var answer = viewSubmission.View.State.Values.First().Value.First().Value.SelectedOption.Value;
                    _logger.LogDebug("Answer: {answer}", answer);

                    var answerEntity = new AnswerEntity(viewSubmission.View.PrivateMetadata, viewSubmission.User.Username)
                    {
                        Answer = answer,
                        Answerer = viewSubmission.User.Username,
                        Question = viewSubmission.View.Title.Text,
                        QuestionnaireId = viewSubmission.View.PrivateMetadata
                    };
                    await _storage.InsertOrMerge(answerEntity);

                    var answeredPayload = PayloadUtility.GetConfirmAnsweredPayload(answer);
                    return new JsonResult(answeredPayload);
                case "delete_a_questionnaire":
                    var questionnaireId = viewSubmission.View.State.Values.First().Value.First().Value.SelectedOption.Value;
                    _logger.LogInformation("Deleting questionnaire with ID: {questionnaire}.", questionnaireId);
                    var questionnaireTitle = await _control.DeleteQuestionnaireAndAnswers(questionnaireId).ConfigureAwait(false);
                    var deletedQuestionnairePayload = PayloadUtility.GetDeletedQuestionnairePayload(questionnaireTitle);
                    return new JsonResult(deletedQuestionnairePayload);
                case "delete_questionnaires":
                    _logger.LogInformation("Deleting all questionnaires and answers.");
                    await _control.DeleteAll();
                    var deletedQuestionnairesPayload = PayloadUtility.GetDeletedQuestionnairesPayload();
                    return new JsonResult(deletedQuestionnairesPayload);
                default:
                    throw new NotImplementedException($"Unknown view callback id: {viewSubmission.View.CallbackId}.");
            }
        }
    }
}
