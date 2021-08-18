using System.Text.Json.Serialization;

namespace SlackLib.Objects
{
    /// <summary>
    /// https://api.slack.com/reference/block-kit/composition-objects#text
    /// 
    /// This is created separately, because if type is "mrkdwn", emoji can't
    /// be serialized.
    /// </summary>
    public class MarkdownTextObject : TextObject
    {
        [JsonPropertyName("type")]
        public override string Type { get; } = "mrkdwn";

        [JsonPropertyName("text")]
        public override string Text { get; set; }
    }
}
