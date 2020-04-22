using Newtonsoft.Json;

namespace SlackLib.Payloads
{
    public class Shortcut : PayloadBase
    {
        [JsonProperty("callback_id")]
        public string CallbackId { get; set; }

        public User User { get; set; }

        [JsonProperty("trigger_id")]
        public string TriggerId { get; set; }
    }
}