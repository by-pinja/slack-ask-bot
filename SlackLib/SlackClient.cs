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
        private readonly SlackClientSettings _slackClientSettings;
        private readonly SlackResponseParser _slackResponseParser;

        public SlackClient(ILogger<SlackClient> logger, SlackClientSettings slackClientSettings, SlackResponseParser slackResponseParser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _slackClientSettings = slackClientSettings ?? throw new ArgumentNullException(nameof(slackClientSettings));
            _slackResponseParser = slackResponseParser ?? throw new ArgumentNullException(nameof(slackResponseParser));
        }

        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_slackClientSettings.BearerToken}");
            return client;
        }

        public async Task PostQuestionaire(string channel, Questionnaire questionnaire)
        {
            using (var client = CreateClient())
            {
                var payload = new
                {
                    channel,
                    text = "Question",
                    blocks = new[]
                    {
                        Section(questionnaire),
                        AnswerOptions(new string[]{"Vastaa"})
                    }
                };
                var serializedPayload = JsonConvert.SerializeObject(payload);
                var uri = new Uri("https://slack.com/api/chat.postMessage");
                using (var response = await client.PostAsync(uri, new StringContent(serializedPayload, Encoding.UTF8, "application/json")))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogTrace("Content: {0}", content);
                    var parsed = _slackResponseParser.Parse(content);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK || !parsed.Ok)
                    {
                        _logger.LogTrace("Status code: {0}. Error message: {0}", response.StatusCode, parsed?.Error);
                        _logger.LogTrace("Content: {0}", content);
                        throw new SlackLibException($"Something went wrong while creating questionnaire. Code was {response.StatusCode}");
                    }
                }
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

        public async Task OpenAnswerDialog(string triggerId, Questionnaire questionnaire)
        {
            using (var client = CreateClient())
            {
                _logger.LogInformation("Opening dialog");
                var payload = new
                {
                    dialog = new
                    {
                        callback_id = questionnaire.QuestionId,
                        title = questionnaire.Question,
                        elements = new[]
                        {
                        new
                        {
                            label = questionnaire.Question,
                            type = "select",
                            name = "answer",
                            options = questionnaire.AnswerOptions.Select(option => {
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
                using (var response = await client.PostAsync(new Uri("https://slack.com/api/dialog.open"), requestContent))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogTrace("Content: {0}", content);
                    var parsed = _slackResponseParser.Parse(content);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK || !parsed.Ok)
                    {
                        _logger.LogTrace("Status code: {0}. Error message: {0}", response.StatusCode, parsed?.Error);
                        throw new SlackLibException($"Something went wrong while responding to answer. Code was {response.StatusCode}");
                    }
                }
            }
        }
    }
}
