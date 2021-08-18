using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// Base class for block elements
    /// 
    /// https://api.slack.com/reference/block-kit/block-elements#button
    /// </summary>
    public abstract class BlockElement
    {
        [JsonProperty("type")]
        public string Type { get; }

        public BlockElement(string type)
        {
            Type = type;
        }
    }
}
