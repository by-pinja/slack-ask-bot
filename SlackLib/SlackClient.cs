using System;
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
    public class SlackClient : ISlackClient
    {
        private readonly ILogger<SlackClient> _logger;
        private readonly HttpClient _client;

        public SlackClient(ILogger<SlackClient> logger, SlackClientSettings slackClientSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var client = new HttpClient();
            //if (_client is null) throw new NullReferenceException("Http client uninitialized in static constructor.");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {slackClientSettings?.BearerToken ?? throw new ArgumentNullException(nameof(slackClientSettings))}");
            _client = client;
        }

        ~SlackClient()
        {
            _client.Dispose();
        }



        public async Task PostQuestionnaire(Questionnaire questionnaire, string channel)
        {
            _logger.LogInformation("Posting questionnaire message.");

            var messagePayload = new
            {
                channel,
                text = "PostQuestionnaire",
                blocks = new[]
                {
                    Section(questionnaire),
                    new
                    {
                        type = "actions",
                        elements = new[]
                        {
                            new
                            {
                                type = "button",
                                value = questionnaire.QuestionId,
                                text = new
                                {
                                    type = "plain_text",
                                    text = "Vastaa"
                                }
                            }
                        }
                    }
                }
            };

            await ExecuteSlackCall(messagePayload, "https://slack.com/api/chat.postMessage");
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

        // public async Task UpdateViewWithAnswers(QuestionnaireResult questionnaireResult, string viewId, string hash)
        // {
        //     _logger.LogInformation("Updating the get answers view.");

        //     await ExecuteSlackCall(viewPayload, "https://slack.com/api/views.update", "opening answers for questionnaire view").ConfigureAwait(false);
        // }

        public async Task PostMessage(dynamic payload)
        {
            await ExecuteSlackCall(payload, "https://slack.com/api/chat.postMessage").ConfigureAwait(false);
        }

        public async Task OpenModelView(dynamic payload)
        {
            await ExecuteSlackCall(payload, "https://slack.com/api/views.open").ConfigureAwait(false);
        }

        public async Task UpdateModelView(dynamic payload)
        {
            await ExecuteSlackCall(payload, "https://slack.com/api/views.update").ConfigureAwait(false);
        }

        private async Task ExecuteSlackCall(dynamic payload, string address)
        {
            _logger.LogDebug("Executing slack request at {address}", address);

            try
            {
                string serializedPayload = JsonConvert.SerializeObject(payload);
                _logger.LogDebug("Serialised: {payload}.", serializedPayload);
                using (var requestContent = new StringContent(serializedPayload, Encoding.UTF8, "application/json"))
                {
                    var response = await _client.PostAsync(address, requestContent).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    _logger.LogTrace("Request successful, checking content. Content: {content}", content);
                    var parsed = JsonConvert.DeserializeObject<dynamic>(content);
                    if (parsed is null || !parsed.Ok)
                    {
                        _logger.LogTrace("Request successful but SlackAPI error. Error message: {error_message}", (string)parsed.Error);
                        throw new SlackLibException($"Error message: {parsed.Error}");
                    }
                }
            }
            catch (JsonSerializationException)
            {
                _logger.LogTrace("Failed to serialise the payload or deserialize the slack response.");
                throw;
            }
            catch (HttpRequestException)
            {
                _logger.LogTrace("Failed Http request to Slack API.");
                throw;
            }
        }
    }
}
