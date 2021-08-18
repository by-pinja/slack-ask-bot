using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/block-kit/blocks#section
    /// </summary>
    public abstract class TextObject
    {
        [JsonProperty("type")]
        public abstract string Type { get; }

        [JsonProperty("text")]
        public abstract string Text { get; set; }
    }
}
