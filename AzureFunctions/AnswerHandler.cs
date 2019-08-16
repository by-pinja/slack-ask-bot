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

namespace AzureFunctions
{
    public class AnswerHandler
    {
        private readonly ILogger<AnswerHandler> _logger;
        private readonly IStorage _storage;
        private readonly SlackClient _slackClient;
        private readonly PayloadParser _payloadParser;

        public AnswerHandler(ILogger<AnswerHandler> logger, IStorage storage, SlackClient slackClient, PayloadParser payloadParser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _slackClient = slackClient ?? throw new ArgumentNullException(nameof(slackClient));
            _payloadParser = payloadParser ?? throw new ArgumentNullException(nameof(payloadParser));
        }

        [FunctionName("AnswerHandler")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req)
        {
            _logger.LogDebug("AnswerHandler hook launched");
            var contentString = await req.Content.ReadAsStringAsync();
            var parsed = _payloadParser.Parse(contentString);

            switch (parsed)
            {
                case AnswerContext answerContext:
                    await HandleAnswerRequest(answerContext);
                    break;
                case DialogOpenRequest dialogRequest:
                    await HandleDialogOpenRequest(dialogRequest);
                    break;
                default:
                    throw new NotImplementedException("Unkown object type.");
            }
        }

        private async Task HandleAnswerRequest(AnswerContext answerContext)
        {
            _logger.LogInformation("Answer received from channel {channel} by {answerer}. Answer: {answer}", answerContext.Channel, answerContext.Answerer, answerContext.Answer);

            var questionnaire = (await _storage.GetQuestionnaires(answerContext.QuestionnaireId)).FirstOrDefault();
            var answer = new AnswerEntity(answerContext.Id, answerContext.Channel)
            {
                Answer = answerContext.Answer,
                Answerer = answerContext.Answerer,
                Channel = answerContext.Channel,
                Question = questionnaire.Question,
                QuestionnaireId = questionnaire.QuestionaireId
            };
            await _storage.InsertOrMerge(answer);
        }

        private async Task HandleDialogOpenRequest(DialogOpenRequest dialogRequest)
        {
            _logger.LogInformation("Dialog open request received from  {channel} by {answerer}", dialogRequest.Channel, dialogRequest.Answerer);
            var dtoQuestionnaire = (await _storage.GetQuestionnaires(dialogRequest.QuestionnaireId)).FirstOrDefault();
            var questionnaire = new Questionnaire()
            {
                QuestionId = dtoQuestionnaire.QuestionaireId,
                Question = dtoQuestionnaire.Question,
                AnswerOptions = dtoQuestionnaire.AnswerOptions.Split(";")
            };
            await _slackClient.OpenAnswerDialog(dialogRequest.Id, questionnaire);
        }
    }
}
