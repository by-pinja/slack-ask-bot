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
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer xoxp-7257967057-10473521619-554554081012-365eeca937f6e027b5960b29ea2f36c3");
        }

        public async Task PostQuestionaire(string webHookUrl, string channel, Questionnaire questionnaire)
        {
            var payload = new
            {
                channel = channel,
                blocks = new []
                {
                    Section(questionnaire),
                    AnswerOptions(new string[]{"Vastaa"})
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

        public async Task OpenAnswerDialog(string triggerId, string question, string questionnaireId)
        {
            _logger.LogInformation("Opening dialog");
            var payload = new
            {
                dialog = new {
                    callback_id = questionnaireId,
                    title = question,
                    elements = new []
                    {
                        new 
                        {
                            label = question,
                            type = "select",
                            name = "answer",
                            options = new []{"test1", "test2", ":töhötys:"}.Select(option => {
                                return new 
                                {
                                    label = option,
                                    value = option
                                };
                            })
                        }
                    }
                },
                trigger_id = triggerId
            };
            var serializedPayload = JsonConvert.SerializeObject(payload);
            var requestContent = new StringContent(serializedPayload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(new Uri("https://slack.com/api/dialog.open"), requestContent);

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Status code: {0}", response.StatusCode);
            _logger.LogInformation("Content: {0}", content);
            if (response.StatusCode != System.Net.HttpStatusCode.OK && content != "ok")
            {
                throw new SlackLibException($"Something went wrong while responding to answer. Code was {response.StatusCode}");
            }
        }
    }
}
