using System.Text.Json.Serialization;

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
        public object View { get; set; }
    }
}
