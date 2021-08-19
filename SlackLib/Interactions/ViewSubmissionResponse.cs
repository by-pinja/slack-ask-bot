using Newtonsoft.Json;
using SlackLib.Objects;

namespace SlackLib.Interactions
{
    /// <summary>
    /// This is returned as a response to view_submission which is received from Slack.
    /// 
    /// https://api.slack.com/surfaces/modals/using#updating_response
    /// </summary>
    public class ViewSubmissionResponse
    {
        [JsonProperty("response_action")]
        public string ResponseAction { get; set; }

        [JsonProperty("view")]
        public ViewObject View { get; set; }
    }
}
