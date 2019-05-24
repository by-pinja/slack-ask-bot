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
    
        public async Task<HttpResponseMessage> PostQuestionaire(string channel, Questionnaire questionnaire)
        {
            var payload = new
            {
                channel = channel,
                blocks = new []
                {
                    Section(questionnaire),
                    AnswerOptions(questionnaire.AnswerOptions)
                }
            };
            var serializedPayload = JsonConvert.SerializeObject(payload);
            var response = await _httpClient.PostAsync(_webhookUrl,
                new StringContent(serializedPayload, Encoding.UTF8, "application/json"));
    
            return response;
        }

        private dynamic Section(Questionnaire questionnaire)
        {
            return new 
            {
                type = "section",
                block_id = questionnaire.QuestionId,
                text = new
                {
                    type = "mrkdwn",
                    text = questionnaire.Question
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
