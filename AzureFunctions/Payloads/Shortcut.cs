using Newtonsoft.Json;

namespace AzureFunctions.Payloads
{
    public class Shortcut
    {
        [JsonProperty("callback_id")]
        public string CallbackId { get; set; }

        public User User { get; set; }

        [JsonProperty("trigger_id")]
        public string TriggerId { get; set; }
    }
}