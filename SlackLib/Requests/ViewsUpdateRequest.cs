using Newtonsoft.Json;
using SlackLib.Objects;

namespace SlackLib.Requests
{
    /// <summary>
    /// https://api.slack.com/methods/views.update
    /// </summary>
    public class ViewsUpdateRequest
    {
        [JsonProperty("view_id")]
        public string ViewId { get; set; }

        [JsonProperty("trigger_id")]
        public string TriggerId { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("view")]
        public ViewObject View { get; set; }
    }
}
