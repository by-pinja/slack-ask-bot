using Newtonsoft.Json;

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
        public MarkdownTextObject() : base("mrkdwn")
        {
        }
    }
}
