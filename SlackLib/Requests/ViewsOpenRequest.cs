using Newtonsoft.Json;
using SlackLib.Objects;

namespace SlackLib.Requests
{
    /// <summary>
    /// https://api.slack.com/methods/views.open
    /// </summary>
    public class ViewsOpenRequest
    {
        [JsonProperty("trigger_id")]
        public string TriggerId { get; set; }

        [JsonProperty("view")]
        public ViewObject View { get; set; }
    }
}
