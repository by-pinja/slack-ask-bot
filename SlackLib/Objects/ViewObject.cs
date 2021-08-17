using System.Text.Json.Serialization;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/surfaces/views
    /// </summary>
    public class ViewObject
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public dynamic Title { get; set; }

        [JsonPropertyName("callback_id")]
        public string CallbackId { get; set; }

        [JsonPropertyName("private_metadata")]
        public string PrivateMetadata { get; set; }

        [JsonPropertyName("submit")]
        public dynamic Submit { get; set; }

        [JsonPropertyName("close")]
        public dynamic Close { get; set; }

        [JsonPropertyName("blocks")]
        public dynamic Blocks { get; set; }
    }
}
