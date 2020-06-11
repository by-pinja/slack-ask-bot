using System.Text.Json.Serialization;

namespace AzureFunctions.Payloads
{
    public class Shortcut
    {
        [JsonPropertyName("callback_id")]
        public string CallbackId { get; set; }

        public User User { get; set; }

        [JsonPropertyName("trigger_id")]
        public string TriggerId { get; set; }
    }
}