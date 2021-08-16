using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SlackLib.Responses;

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

        public async Task<ChatPostMessageResponse> PostMessage(dynamic payload)
        {
            return await ExecuteSlackCall<ChatPostMessageResponse>(payload, "chat.postMessage").ConfigureAwait(false);
        }

        public async Task ChatUpdate(dynamic payload)
        {
            await ExecuteSlackCall<ChatPostMessageResponse>(payload, "chat.update").ConfigureAwait(false);
        }

        public async Task OpenModelView(dynamic payload)
        {
            await ExecuteSlackCall<object>(payload, "views.open").ConfigureAwait(false);
        }
        public async Task UpdateModelView(dynamic payload)
        {
            await ExecuteSlackCall<object>(payload, "views.update").ConfigureAwait(false);
        }

        private async Task<T> ExecuteSlackCall<T>(dynamic payload, string address)
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

                    _logger.LogDebug("Request successful, checking content. Content: {content}", content);
                    var parsed = JsonSerializer.Deserialize<JsonElement>(content);
                    if (!parsed.GetProperty("ok").GetBoolean())
                    {
                        _logger.LogCritical("Request successful but SlackAPI error. Error message: {error_message}", parsed.GetProperty("error").GetString());
                        throw new SlackLibException($"Error message: {parsed.GetProperty("error").GetString()}");
                    }
                    return JsonSerializer.Deserialize<T>(content);
                }
            }
            catch (JsonException e)
            {
                _logger.LogCritical(e, "Failed to serialise the payload or deserialize the slack response.");
                throw new SlackLibException("Json serialising error.", e);
            }
            catch (HttpRequestException e)
            {
                _logger.LogCritical(e, "Failed Http request to Slack API.");
                throw new SlackLibException("Http request error.", e);
            }
        }
    }
}
