using Newtonsoft.Json;

namespace SlackLib.Payloads
{
    public class BlockActions : PayloadBase
    {
        [JsonProperty("trigger_id")]
        public string TriggerId { get; set; }

        public Message Message { get; set; }

        public Channel Channel { get; set; }

        public MessagingUser User { get; set; }

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

    /// <summary>
    /// For some very nice reason this is actually different type than the user received by dialog submission.
    /// </summary>
    public class MessagingUser
    {
        public string Username { get; set; }
    }
}