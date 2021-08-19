using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/block-kit/block-elements#button
    /// </summary>
    public class ButtonElement : BlockElement
    {
        [JsonProperty("action_id")]
        public string ActionId { get; set; }

        [JsonProperty("text")]
        public PlainTextObject Text { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public ButtonElement() : base("button")
        {
        }
    }
}
