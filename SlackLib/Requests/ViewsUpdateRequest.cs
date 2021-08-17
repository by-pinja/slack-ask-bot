using System.Text.Json.Serialization;

namespace SlackLib.Requests
{
    /// <summary>
    /// https://api.slack.com/methods/views.update
    /// </summary>
    public class ViewsUpdateRequest
    {
        [JsonPropertyName("view_id")]
        public string ViewId { get; set; }

        [JsonPropertyName("external_id")]
        public string ExternalId { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        [JsonPropertyName("view")]
        public object View { get; set; }
    }
}
