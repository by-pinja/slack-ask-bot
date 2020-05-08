using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AskBotCore;
using CloudLib;
using CloudLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SlackLib;
using SlackLib.Messages;
using SlackLib.Payloads;

namespace AzureFunctions
{
    public class AskBotHandler
    {
        private readonly ILogger<AskBotHandler> _logger;
        private readonly IStorage _storage;
        private readonly SlackClient _slackClient;
        private readonly PayloadParser _payloadParser;
        private readonly AskBotControl _control;

        public AskBotHandler(ILogger<AskBotHandler> logger, IStorage storage, SlackClient slackClient, PayloadParser payloadParser, AskBotControl control)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _slackClient = slackClient ?? throw new ArgumentNullException(nameof(slackClient));
            _payloadParser = payloadParser ?? throw new ArgumentNullException(nameof(payloadParser));
            _control = control ?? throw new ArgumentNullException(nameof(control));
        }

        [FunctionName(nameof(AskBotHook))]
        public async Task<IActionResult> AskBotHook([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req)
        {
            _logger.LogDebug("AskBot hook launched");
            var contentString = await req.Content.ReadAsStringAsync();
            var parsed = _payloadParser.Parse(contentString);

            switch (parsed)
            {
                case BlockActions dialogRequest:
                    await HandleQuestionnaireOpenRequest(dialogRequest);
                    break;
                case Shortcut questionnaireRequest:
                    await HandleShortcutRequest(questionnaireRequest);
                    break;
                case ViewSubmission viewSubmission:
                    await HandleViewSubmission(viewSubmission);
                    break;
                default:
                    throw new NotImplementedException("Unknown object type.");
            }

            return new OkResult();
        }

        private async Task HandleQuestionnaireOpenRequest(BlockActions blockActions)
        {
            _logger.LogInformation("Dialog open request received from  {channel} by {answerer}", blockActions.Channel, blockActions.User.Username);
            var dtoQuestionnaire = (await _storage.GetQuestionnaires(blockActions.Actions[0].Value)).FirstOrDefault();

            if (dtoQuestionnaire is null)
            {
                _logger.LogCritical("Error retrieving the questionnaire for callback id: {callbackId}.", blockActions.Actions[0].Value);
                throw new Exception("Can not retrieve the correct questionnaire.");
            }

            var questionnaire = new Questionnaire()
            {
                QuestionId = dtoQuestionnaire.QuestionnaireId,
                Question = dtoQuestionnaire.Question,
                AnswerOptions = dtoQuestionnaire.AnswerOptions.Split(";")
            };
            await _slackClient.OpenAnswerView(blockActions.TriggerId, questionnaire);
        }

        private async Task HandleShortcutRequest(Shortcut shortcutRequest)
        {
            _logger.LogInformation("Shortcut request received from {user} with callback ID: {callback}", shortcutRequest.User.Username, shortcutRequest.CallbackId);

            switch (shortcutRequest.CallbackId)
            {
                case "create_questionnaire":
                    await _slackClient.OpenCreateQuestionnaireView(shortcutRequest.TriggerId);
                    break;
                case "get_answers":
                    var questionnaires = await _control.GetQuestionnaires().ConfigureAwait(false);
                    await _slackClient.OpenGetAnswersView(questionnaires, shortcutRequest.TriggerId);
                    break;
                default:
                    throw new NotImplementedException($"Unknown shortcut callback id: {shortcutRequest.CallbackId}.");
            }
        }

        private async Task HandleViewSubmission(ViewSubmission viewSubmission)
        {
            _logger.LogInformation("Entered view submission.");

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

                    // var answeredQuestionnaire = (await _storage.GetQuestionnaires(viewSubmission.View.PrivateMetadata)).FirstOrDefault();
                    // if (answeredQuestionnaire is null)
                    // {
                    //     _logger.LogError("Error retrieving the questionnaire for callback id: {callbackId}.", viewSubmission.View.CallbackId);
                    //     return;
                    // }

                    var answerEntity = new AnswerEntity(viewSubmission.View.PrivateMetadata, viewSubmission.User.Username)
                    {
                        Answer = answer,
                        Answerer = viewSubmission.User.Username,
                        //Channel = submission.Channel.Name,
                        Question = viewSubmission.View.Title.Text,
                        QuestionnaireId = viewSubmission.View.PrivateMetadata
                    };
                    await _storage.InsertOrMerge(answerEntity);
                    break;
                case "get_answers":
                    var selectedQuestionnaireId = viewSubmission.View.State.values.First().Value.First().Value.SelectedOption.Value;
                    _logger.LogInformation("Get answers for questionnaire with ID: {questionnaire}.", selectedQuestionnaireId);
                    var questionnaireResult = await _control.GetAnswers(selectedQuestionnaireId).ConfigureAwait(false);
                    await _slackClient.UpdateViewWithAnswers(questionnaireResult, viewSubmission.View.Id, viewSubmission.View.Hash);
                    break;
                default:
                    throw new NotImplementedException($"Unknown view callback id: {viewSubmission.View.CallbackId}.");
            }
        }
    }
}

