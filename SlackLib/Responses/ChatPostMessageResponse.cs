using Newtonsoft.Json;

namespace SlackLib.Responses
{
    public class ChatPostMessageResponse
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("ts")]
        public string Timestamp { get; set; }
    }
}
