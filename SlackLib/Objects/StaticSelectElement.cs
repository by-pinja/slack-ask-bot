using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/block-kit/block-elements#select
    /// </summary>
    public class StaticSelectElement : BlockElement
    {
        [JsonProperty("action_id")]
        public string ActionId { get; set; }

        [JsonProperty("placeholder")]
        public PlainTextObject Placeholder { get; set; }

        [JsonProperty("options")]
        public OptionObject[] Options { get; set; }

        public StaticSelectElement() : base("static_select")
        {
        }
    }

    /// <summary>
    /// https://api.slack.com/reference/block-kit/composition-objects#option
    /// </summary>
    public class OptionObject
    {
        [JsonProperty("text")]
        public TextObject Text { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
