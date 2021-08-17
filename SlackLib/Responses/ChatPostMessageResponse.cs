using System.Text.Json.Serialization;

namespace SlackLib.Responses
{
    public class ChatPostMessageResponse
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("ts")]
        public string Timestamp { get; set; }
    }
}
