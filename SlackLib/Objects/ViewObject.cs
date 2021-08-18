using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/surfaces/views
    /// </summary>
    public class ViewObject
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title")]
        public PlainTextObject Title { get; set; }

        [JsonProperty("callback_id")]
        public string CallbackId { get; set; }

        [JsonProperty("private_metadata")]
        public string PrivateMetadata { get; set; }

        [JsonProperty("submit")]
        public PlainTextObject Submit { get; set; }

        [JsonProperty("close")]
        public PlainTextObject Close { get; set; }

        [JsonProperty("blocks")]
        public dynamic Blocks { get; set; }
    }
}
