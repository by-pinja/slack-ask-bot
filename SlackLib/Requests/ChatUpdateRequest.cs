using Newtonsoft.Json;

namespace SlackLib.Requests
{
    /// <summary>
    /// https://api.slack.com/methods/chat.update
    /// </summary>
    public class ChatUpdateRequest
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("ts")]
        public string Timestamp { get; set; }

        [JsonProperty("blocks")]
        public object[] Blocks { get; set; }
    }
}
