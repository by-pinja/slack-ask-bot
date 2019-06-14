using System.Threading.Tasks;
using CloudLib;
using Microsoft.Extensions.Logging;
using SlackLib;
using SlackLib.Messages;

namespace ConsoleTester
{
    /// <summary>
    /// Because "new-style" Incoming Webhooks currently only supports single channel access,
    /// this class tries to hide that fact from user. Slack client could use "old-style" API
    /// which would support multi-channel access, but this is avoided for now.
    /// </summary>
    public class SlackWrapper
    {
        private readonly ILogger<SlackWrapper> _logger;
        private readonly SlackClient _client;
        private readonly IStorage _storage;

        public SlackWrapper(ILogger<SlackWrapper> logger, SlackClient slackClient, Storage storage)
        {
            _logger = logger;
            _client = slackClient;
            _storage = storage;
        } 

        public async Task SendQuestionaire(string channel, Questionnaire questionnaire)
        {
            var webHook = await _storage.GetChannelWebHook(channel);
            if (webHook == null)
            {
                throw new ChannelWebHookMissingException($"No beb hook found for channel {channel}. Unable to send message to channel.");
            }

            await _client.PostQuestionaire(webHook.Webhook, channel, questionnaire);
        }
    }
}