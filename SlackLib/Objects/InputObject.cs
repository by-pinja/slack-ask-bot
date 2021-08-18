using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/block-kit/blocks#input
    /// </summary>
    public class InputObject : BlockObject
    {
        [JsonProperty("element")]
        public BlockElement Element { get; set; }

        [JsonProperty("label")]
        public PlainTextObject Label { get; set; }

        public InputObject() : base("input")
        {

        }
    }
}
