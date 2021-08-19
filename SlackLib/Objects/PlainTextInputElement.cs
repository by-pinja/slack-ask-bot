using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/block-kit/block-elements#input
    /// </summary>
    public class PlainTextInputElement : BlockElement
    {
        [JsonProperty("action_id")]
        public string ActionId { get; set; }

        [JsonProperty("placeholder")]
        public PlainTextObject Placeholder { get; set; }

        [JsonProperty("max_length")]
        public int MaxLength { get; set; }

        public PlainTextInputElement() : base("plain_text_input")
        {
        }
    }
}
