using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// Block element containing actions
    /// 
    /// https://api.slack.com/reference/block-kit/blocks#actions
    /// </summary>
    public class ActionBlock : BlockObject
    {
        public ActionBlock() : base("actions")
        {
        }

        [JsonProperty("elements")]
        public BlockElement[] Elements { get; set; }
    }
}
