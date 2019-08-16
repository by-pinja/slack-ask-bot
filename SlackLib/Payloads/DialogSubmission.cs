using Newtonsoft.Json;

namespace SlackLib.Payloads
{
    public class DialogSubmission : PayloadBase
    {
        [JsonProperty("action_ts")]
        public string ActionTimestamp { get; set; }

        [JsonProperty("callback_id")]
        public string CallbackId { get; set; }

        public Channel Channel { get; set; }

        public DialogSubmissionUser User { get; set; }

        public Submission Submission { get; set; }

        [JsonProperty("response_url")]
        public string ResponseUrl { get; set; }
    }

    public class DialogSubmissionUser
    {
        public string Name { get; set; }
    }

    public class Submission
    {
        public string Answer { get; set; }
    }
}