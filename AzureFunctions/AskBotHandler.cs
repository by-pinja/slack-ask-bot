﻿using System;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json;
using SlackLib;
using SlackLib.Messages;

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
                throw new ArgumentException("Content of HttpRequest is empty.");
            }

            _logger.LogTrace("Parsing raw: {content}", contentString);
            var escaped = HttpUtility.ParseQueryString(contentString);
            var payloadString = escaped["payload"];
            if (string.IsNullOrWhiteSpace(payloadString))
            {
                throw new ArgumentException("No payload element found in content.");
            }

            _logger.LogDebug("Deserializing payload: {payload}", payloadString);
            var json = JsonConvert.DeserializeObject<dynamic>(payloadString);

            switch ((string)json.type)
            {
                case "block_actions":
                    var blockAction = JsonConvert.DeserializeObject<BlockActions>(payloadString);
                    return await HandleBlockAction(blockAction);
                case "shortcut":
                    var shortcut = JsonConvert.DeserializeObject<Shortcut>(payloadString);
                    return await HandleShortcut(shortcut);
                case "view_submission":
                    var viewSubmission = JsonConvert.DeserializeObject<ViewSubmission>(payloadString);
                    return await HandleViewSubmission(viewSubmission);
                default:
                    throw new NotImplementedException($"Unknown payload type {json.type}.");
            }
        }

        public async Task<IActionResult> HandleBlockAction(BlockActions blockAction)
        {
            _logger.LogInformation("Questionnaire open request received from {channel} by {answerer}", blockAction.Channel.Name, blockAction.User.Username);
            var dtoQuestionnaire = await _storage.GetQuestionnaire(blockAction.Actions[0].Value);

            dynamic viewPayload;
            if (dtoQuestionnaire is null)
            {
                _logger.LogDebug("Error retrieving the questionnaire for callback id: {callbackId}.", blockAction.Actions[0].Value);
                viewPayload = blockAction.GetRemovedQuestionnaireViewPayload();
            }
            else
            {
                var questionnaire = new Questionnaire()
                {
                    QuestionId = dtoQuestionnaire.QuestionnaireId,
                    Question = dtoQuestionnaire.Question,
                    AnswerOptions = dtoQuestionnaire.AnswerOptions.Split(";")
                };

                viewPayload = blockAction.GetOpenQuestionnaireViewPayload(questionnaire);
            }

            _logger.LogInformation("Opening slack model to answer the questionnaire.");
            await _slackClient.OpenModelView(viewPayload);

            return new OkResult();
        }

        public async Task<IActionResult> HandleShortcut(Shortcut shortcut)
        {
            _logger.LogInformation("Shortcut request received from {user} with callback ID: {callback}", shortcut.User.Username, shortcut.CallbackId);

            dynamic payload;
            switch (shortcut.CallbackId)
            {
                case "create_questionnaire":
                    payload = shortcut.GetOpenCreateQuestionnairesPayload();

                    _logger.LogInformation("Opening slack model to create questionnaire.");
                    await _slackClient.OpenModelView(payload);
                    break;
                case "get_answers":
                case "delete_a_questionnaire":
                    _logger.LogInformation("Fetching questionnaires.");
                    var questionnaires = await _storage.GetQuestionnaires().ConfigureAwait(false);
                    if (questionnaires is null || questionnaires.Count() == 0)
                    {
                        payload = shortcut.GetNoQuestionnairesAvailablePayload();
                    }
                    else
                    {
                        payload = shortcut.GetOpenListOfQuestionnairesPayload(questionnaires, shortcut.CallbackId);
                    }

                    _logger.LogInformation("Opening slack model to list the questionnaires available.");
                    await _slackClient.OpenModelView(payload);
                    break;
                case "delete_questionnaires":
                    _logger.LogInformation("Send cornfirmation view.");
                    payload = shortcut.GetConfirmDeleteAllPayload();
                    await _slackClient.OpenModelView(payload);
                    break;
                default:
                    throw new NotImplementedException($"Unknown shortcut callback id: {shortcut.CallbackId}.");
            }

            return new OkResult();
        }

        public async Task<IActionResult> HandleViewSubmission(ViewSubmission viewSubmission)
        {
            _logger.LogInformation("View submission received.");

            switch (viewSubmission.View.CallbackId)
            {
                case "create_questionnaire":
                    _logger.LogInformation("Creating and posting questionnaire received from {user}.", viewSubmission.User.Username);

                    var channel = viewSubmission.View.State.values["ChannelBlock"].First().Value.Value;
                    if (string.IsNullOrWhiteSpace(channel))
                    {
                        throw new Exception("channel is null");
                    }
                    var question = viewSubmission.View.State.values["TitleBlock"]["title"].Value;
                    if (string.IsNullOrWhiteSpace(question))
                    {
                        throw new Exception("question is null");
                    }

                    var answerOptionDictionaries = viewSubmission.View.State.values.Where(d => d.Key.Contains("Answer")).Select(kvp => kvp.Value);
                    var answerOptions = answerOptionDictionaries.Select(d => d.First().Value.Value).ToArray();

                    if (answerOptions.Count() == 0)
                    {
                        throw new Exception("answer option is empty.");
                    }

                    var questionnaire = new Questionnaire
                    {
                        QuestionId = Guid.NewGuid().ToString(),
                        Question = question,
                        AnswerOptions = answerOptions
                    };

                    await _control.CreateQuestionnaire(questionnaire, channel, DateTime.UtcNow).ConfigureAwait(false);
                    break;
                case "open_questionnaire":
                    _logger.LogInformation("Answer received from {answerer}.", viewSubmission.User.Username);
                    var answer = viewSubmission.View.State.values.First().Value.First().Value.SelectedOption.Value;
                    _logger.LogDebug("Answer: {answer}", answer);

                    var answerEntity = new AnswerEntity(viewSubmission.View.PrivateMetadata, viewSubmission.User.Username)
                    {
                        Answer = answer,
                        Answerer = viewSubmission.User.Username,
                        //Channel = submission.Channel.Name,
                        Question = viewSubmission.View.Title.Text,
                        QuestionnaireId = viewSubmission.View.PrivateMetadata
                    };
                    await _storage.InsertOrMerge(answerEntity);

                    var answeredPayload = viewSubmission.GetConfirmAnsweredPayload();
                    return new JsonResult(answeredPayload);
                case "get_answers":
                    var selectedQuestionnaireId = viewSubmission.View.State.values.First().Value.First().Value.SelectedOption.Value;
                    _logger.LogInformation("Getting answers for questionnaire with ID: {questionnaire}.", selectedQuestionnaireId);
                    var questionnaireResult = await _control.GetQuestionnaireResult(selectedQuestionnaireId).ConfigureAwait(false);
                    var withAnswersPayload = viewSubmission.GetUpdateModelWithAnswersPayload(questionnaireResult);
                    return new JsonResult(withAnswersPayload);
                case "delete_a_questionnaire":
                    var questionnaireId = viewSubmission.View.State.values.First().Value.First().Value.SelectedOption.Value;
                    _logger.LogInformation("Deleting questionnaire with ID: {questionnaire}.", questionnaireId);
                    var questionnaireTitle = await _control.DeleteQuestionnaireAndAnswers(questionnaireId).ConfigureAwait(false);
                    var deletedQuestionnairePayload = viewSubmission.GetDeletedQuestionnairePayload(questionnaireTitle);
                    return new JsonResult(deletedQuestionnairePayload);
                case "delete_questionnaires":
                    _logger.LogInformation("Deleting all questionnaires and answers.");
                    await _control.DeleteAll();
                    var deletedQuestionnairesPayload = viewSubmission.GetDeletedQuestionnairesPayload();
                    return new JsonResult(deletedQuestionnairesPayload);
                default:
                    throw new NotImplementedException($"Unknown view callback id: {viewSubmission.View.CallbackId}.");
            }

            return new OkResult();
        }
    }
}