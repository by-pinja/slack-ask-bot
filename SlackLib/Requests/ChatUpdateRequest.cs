using System.Text.Json.Serialization;

namespace SlackLib.Requests
{
    /// <summary>
    /// https://api.slack.com/methods/chat.update
    /// </summary>
    public class ChatUpdateRequest
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("ts")]
        public string Timestamp { get; set; }

        [JsonPropertyName("blocks")]
        public object[] Blocks { get; set; }
    }
}
