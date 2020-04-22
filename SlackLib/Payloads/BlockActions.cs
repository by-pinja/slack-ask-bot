using Newtonsoft.Json;

namespace SlackLib.Payloads
{
    public class BlockActions : PayloadBase
    {
        [JsonProperty("trigger_id")]
        public string TriggerId { get; set; }

        public Message Message { get; set; }

        public Channel Channel { get; set; }

        public User User { get; set; }

        [JsonProperty("response_url")]
        public string ResponseUrl { get; set; }
    }

    public class Message
    {
        public Block[] Blocks { get; set; }
    }

    public class Block
    {
        [JsonProperty("block_id")]
        public string BlockId { get; set; }
    }
}