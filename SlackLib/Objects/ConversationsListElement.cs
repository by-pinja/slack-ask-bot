using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/block-kit/block-elements#conversation_select
    /// </summary>
    public class ConversationsListElement : BlockElement
    {
        [JsonProperty("action_id")]
        public string ActionId { get; set; }

        [JsonProperty("placeholder")]
        public PlainTextObject Placeholder { get; set; }

        public ConversationsListElement() : base("conversations_select")
        {
        }
    }
}
