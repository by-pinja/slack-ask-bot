using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CloudLib;
using CloudLib.Models;
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

        public AskBotHandler(ILogger<AskBotHandler> logger, IStorage storage, SlackClient slackClient, PayloadParser payloadParser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _slackClient = slackClient ?? throw new ArgumentNullException(nameof(slackClient));
            _payloadParser = payloadParser ?? throw new ArgumentNullException(nameof(payloadParser));
        }

        [FunctionName(nameof(AskBotHook))]
        public async Task AskBotHook([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req)
        {
            _logger.LogDebug("AnswerHandler hook launched");
            var contentString = await req.Content.ReadAsStringAsync();
            var parsed = _payloadParser.Parse(contentString);

            switch (parsed)
            {
                case DialogSubmission answerContext:
                    await HandleAnswerRequest(answerContext);
                    break;
                case BlockActions dialogRequest:
                    await HandleDialogOpenRequest(dialogRequest);
                    break;
                case Shortcut questionnaireRequest:
                    await HandleShortcutRequest(questionnaireRequest);
                    break;
                default:
                    throw new NotImplementedException("Unknown object type.");
            }
        }

        private async Task HandleAnswerRequest(DialogSubmission submission)
        {
            _logger.LogInformation("Answer received from channel {channel} by {answerer}. Answer: {answer}", submission.Channel, submission.User.Name, submission.Submission.Answer);

            var questionnaire = (await _storage.GetQuestionnaires(submission.CallbackId)).FirstOrDefault();

            if (questionnaire is null)
            {
                _logger.LogError("Error retrieving the questionnaire for callback id: {callbackId}.", submission.CallbackId);
                return;
            }

            var answer = new AnswerEntity(submission.ActionTimestamp, submission.Channel.Name)
            {
                Answer = submission.Submission.Answer,
                Answerer = submission.User.Name,
                Channel = submission.Channel.Name,
                Question = questionnaire.Question,
                QuestionnaireId = questionnaire.QuestionnaireId
            };
            await _storage.InsertOrMerge(answer);
        }

        private async Task HandleDialogOpenRequest(BlockActions blockActions)
        {
            _logger.LogInformation("Dialog open request received from  {channel} by {answerer}", blockActions.Channel, blockActions.User.Username);
            var dtoQuestionnaire = (await _storage.GetQuestionnaires(blockActions.Message.Blocks[0].BlockId)).FirstOrDefault();
            var questionnaire = new Questionnaire()
            {
                QuestionId = dtoQuestionnaire.QuestionnaireId,
                Question = dtoQuestionnaire.Question,
                AnswerOptions = dtoQuestionnaire.AnswerOptions.Split(";")
            };
            await _slackClient.OpenAnswerDialog(blockActions.TriggerId, questionnaire);
        }

        private async Task HandleShortcutRequest(Shortcut shortcutRequest)
        {
            _logger.LogInformation("Shortcut request received from {user} with callback ID: {callback}", shortcutRequest.User.Username, shortcutRequest.CallbackId);
        }
    }
}
