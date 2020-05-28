using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

        public SlackClient(ILogger<SlackClient> logger, HttpClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task PostMessage(dynamic payload)
        {
            await ExecuteSlackCall(payload, "chat.postMessage").ConfigureAwait(false);
        }

        public async Task OpenModelView(dynamic payload)
        {
            await ExecuteSlackCall(payload, "views.open").ConfigureAwait(false);
        }
        public async Task UpdateModelView(dynamic payload)
        {
            await ExecuteSlackCall(payload, "views.update").ConfigureAwait(false);
        }

        private async Task ExecuteSlackCall(dynamic payload, string address)
        {
            _logger.LogDebug("Executing slack request at {address}", address);

            try
            {
                string serializedPayload = JsonSerializer.Serialize(payload);
                _logger.LogDebug("Serialised: {payload}.", serializedPayload);
                using (var requestContent = new StringContent(serializedPayload, Encoding.UTF8, "application/json"))
                {
                    var response = await _client.PostAsync(address, requestContent).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    _logger.LogTrace("Request successful, checking content. Content: {content}", content);
                    var parsed = JsonSerializer.Deserialize<SlackResponse>(content);
                    if (parsed is null || !parsed.ok)
                    {
                        _logger.LogCritical("Request successful but SlackAPI error. Error message: {error_message}", parsed.error);
                        throw new SlackLibException($"Error message: {parsed.error}");
                    }
                }
            }
            catch (JsonException)
            {
                _logger.LogCritical("Failed to serialise the payload or deserialize the slack response.");
                throw;
            }
            catch (HttpRequestException)
            {
                _logger.LogCritical("Failed Http request to Slack API.");
                throw;
            }
        }
    }
}
