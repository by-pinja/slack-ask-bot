using Newtonsoft.Json;

namespace SlackLib.Requests
{
    /// <summary>
    /// https://api.slack.com/methods/chat.postMessage
    /// </summary>
    public class ChatPostMessageRequest
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("thread_ts")]
        public string ThreadTimestamp { get; set; }
    }
}
