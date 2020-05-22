using System.Text.Json.Serialization;

namespace AzureFunctions.Payloads
{
    public class BlockAction
    {
        [JsonPropertyName("trigger_id")]
        public string TriggerId { get; set; }

        public WithText Message { get; set; }

        public Channel Channel { get; set; }

        public User User { get; set; }

        public Action[] Actions { get; set; }
    }

    public class Action
    {
        [JsonPropertyName("action_id")]
        public string ActionId { get; set; }

        [JsonPropertyName("block_id")]
        public string BlockId { get; set; }
        public string Value { get; set; }
    }
}