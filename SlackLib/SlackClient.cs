using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackLib.Messages;

namespace SlackLib
{
    public class SlackClient
    {
        private readonly Uri _webhookUrl;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly SlackConfiguration _config;

        public SlackClient(SlackConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config), "config is missing");
            }
            if (config.WebHookUrl == null)
            {
                throw new ArgumentNullException(nameof(config.WebHookUrl), "config.WebHookUrl is missing");
            }

            _config = config;
            _webhookUrl = new Uri(_config.WebHookUrl);
        }
    
        public async Task<HttpResponseMessage> PostQuestionaire(Questionaire questionaire)
        {
            var payload = new
            {
                blocks = new []
                {
                    Section(questionaire),
                    AnswerOptions(questionaire.Answers)
                }
            };
            var serializedPayload = JsonConvert.SerializeObject(payload);
            var response = await _httpClient.PostAsync(_webhookUrl,
                new StringContent(serializedPayload, Encoding.UTF8, "application/json"));
    
            return response;
        }

        private dynamic Section(Questionaire questionaire)
        {
            return new 
            {
                type = "section",
                block_id = questionaire.QuestionId,
                text = new
                {
                    type = "mrkdwn",
                    text = questionaire.Question
                }
            };
        }

        private dynamic AnswerOptions(string[] options)
        {
            return new 
            {
                type = "actions",
                elements = options.Select(option => new
                {
                        type = "button",
                        text = new
                        {
                            type = "plain_text",
                            text = option,
                            emoji = true
                        }
                }).ToArray()
            };
        }
    }
}
