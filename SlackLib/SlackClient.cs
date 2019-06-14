using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackLib.Messages;
using Microsoft.Extensions.Logging;

namespace SlackLib
{
    /// <summary>
    /// Wrapper for Slack API
    /// </summary>
    public class SlackClient
    {
        private readonly ILogger<SlackClient> _logger;
        private readonly HttpClient _httpClient = new HttpClient();

        public SlackClient(ILogger<SlackClient> logger)
        {
            _logger = logger;
        }
    
        public async Task PostQuestionaire(string webHookUrl, string channel, Questionnaire questionnaire)
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
            var response = await _httpClient.PostAsync(new Uri(webHookUrl),
                new StringContent(serializedPayload, Encoding.UTF8, "application/json"));

            var content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK && content != "ok")
            {
                _logger.LogTrace("Status code: {0}", response.StatusCode);
                _logger.LogTrace("Content: {0}", content);
                throw new SlackLibException($"Something went wrong while creating questionnaire. Code was {response.StatusCode}");
            }
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
