using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/block-kit/blocks#section
    /// </summary>
    public abstract class TextObject
    {
        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("text")]
        public string Text { get; set; }

        public TextObject(string type)
        {
            Type = type;
        }
    }
}
