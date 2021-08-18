using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// Base class for all block objects
    /// 
    /// https://api.slack.com/reference/block-kit/blocks
    /// </summary>
    public abstract class BlockObject
    {
        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("block_id")]
        public string BlockId { get; set; }

        public BlockObject(string type)
        {
            Type = type;
        }
    }
}
