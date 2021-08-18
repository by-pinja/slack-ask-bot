using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/block-kit/blocks#section
    /// </summary>
    public class SectionObject
    {
        [JsonProperty("type")]
        [JsonPropertyName("type")]
        public string Type { get; } = "section";

        [JsonProperty("text")]
        [JsonPropertyName("text")]
        public TextObject Text { get; set; }

        [JsonProperty("block_id")]
        [JsonPropertyName("block_id")]
        public string BlockId { get; set; }
    }
}
