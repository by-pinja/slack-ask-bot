using Newtonsoft.Json;

namespace SlackLib.Interactions
{
    /// <summary>
    /// https://api.slack.com/reference/interaction-payloads/block-actions
    /// </summary>
    public class BlockAction
    {
        [JsonProperty("trigger_id")]
        public string TriggerId { get; set; }

        public WithText Message { get; set; }

        public Channel Channel { get; set; }

        public User User { get; set; }

        public Action[] Actions { get; set; }

        public View View { get; set; }
    }

    public class Action
    {
        [JsonProperty("action_id")]
        public string ActionId { get; set; }

        [JsonProperty("block_id")]
        public string BlockId { get; set; }
        public string Value { get; set; }
    }
}
