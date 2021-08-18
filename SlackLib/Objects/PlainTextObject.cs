using Newtonsoft.Json;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/block-kit/composition-objects#text
    /// 
    /// This is created separately, because if type is "mrkdwn", emoji can't
    /// be serialized.
    /// </summary>
    public class PlainTextObject : TextObject
    {
        [JsonProperty("type")]
        public override string Type { get; } = "plain_text";

        [JsonProperty("text")]
        public override string Text { get; set; }

        [JsonProperty("emoji")]
        public bool Emoji { get; set; }
    }
}
