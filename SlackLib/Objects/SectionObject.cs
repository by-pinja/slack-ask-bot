using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/block-kit/blocks#section
    /// </summary>
    public class SectionObject : BlockObject
    {
        [JsonProperty("text")]
        public TextObject Text { get; set; }

        public SectionObject() : base("section")
        {
        }
    }
}
