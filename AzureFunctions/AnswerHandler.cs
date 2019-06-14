using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CloudLib;
using CloudLib.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public class AnswerHandler
    {
        private readonly IStorage _storage;
        public AnswerHandler(IStorage storage)
        {
            _storage = storage;
        }

        [FunctionName("AnswerHandler")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogDebug("AnswerHandler hook launched");
            var contentString = await req.Content.ReadAsStringAsync();
            var parsed = new PayloadParser().Parse(contentString);
            
            log.LogInformation("Answer receivd from channel {0} by {1}. Answer: {2}", parsed.Channel, parsed.Answerer, parsed.Answer);

            var questionnaire = (await _storage.GetQuestionnaires(parsed.QuestionnaireId)).FirstOrDefault();
            var answer = new AnswerEntity(parsed.Id, parsed.Channel)
            {
                Answer = parsed.Answer,
                Answerer = parsed.Answerer,
                Channel = parsed.Channel,
                Question = questionnaire.Question,
                QuestionnaireId = questionnaire.QuestionaireId
            };
            await _storage.InsertOrMerge(answer);
        }
    }
}
