using System.Text.Json.Serialization;
using SlackLib.Objects;

namespace SlackLib.Requests
{
    /// <summary>
    /// https://api.slack.com/methods/views.open
    /// </summary>
    public class ViewsOpenRequest
    {
        [JsonPropertyName("trigger_id")]
        public string TriggerId { get; set; }

        [JsonPropertyName("view")]
        public ViewObject View { get; set; }
    }
}
