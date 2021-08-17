using System.Text.Json.Serialization;

namespace SlackLib.Requests
{
    /// <summary>
    /// https://api.slack.com/methods/chat.postMessage
    /// </summary>
    public class ChatPostMessageRequest
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("thread_ts")]
        public string ThreadTimestamp { get; set; }
    }
}
