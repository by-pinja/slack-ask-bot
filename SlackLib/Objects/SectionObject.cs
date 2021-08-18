using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/block-kit/blocks#section
    /// </summary>
    public class SectionObject
    {
        [JsonProperty("type")]
        public string Type { get; } = "section";

        [JsonProperty("text")]
        public TextObject Text { get; set; }

        [JsonProperty("block_id")]
        public string BlockId { get; set; }
    }
}
