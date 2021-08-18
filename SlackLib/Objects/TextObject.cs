using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/block-kit/blocks#section
    /// </summary>
    public abstract class TextObject
    {
        [JsonProperty("type")]
        [JsonPropertyName("type")]
        public abstract string Type { get; }

        [JsonProperty("text")]
        [JsonPropertyName("text")]
        public abstract string Text { get; set; }
    }
}
