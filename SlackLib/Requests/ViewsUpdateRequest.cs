using System.Text.Json.Serialization;
using SlackLib.Objects;

namespace SlackLib.Requests
{
    /// <summary>
    /// https://api.slack.com/methods/views.update
    /// </summary>
    public class ViewsUpdateRequest
    {
        [JsonPropertyName("view_id")]
        public string ViewId { get; set; }

        [JsonPropertyName("trigger_id")]
        public string TriggerId { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        [JsonPropertyName("view")]
        public ViewObject View { get; set; }
    }
}
