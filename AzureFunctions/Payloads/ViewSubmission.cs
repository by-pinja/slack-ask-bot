using System.Collections.Generic;
using Newtonsoft.Json;

namespace AzureFunctions.Payloads
{
    public class ViewSubmission
    {
        public View View { get; set; }
        public User User { get; set; }
    }

    public class View
    {
        public State State { get; set; }
        public string Id { get; set; }
        public string Hash { get; set; }

        public WithText Title { get; set; }

        [JsonProperty("callback_id")]
        public string CallbackId { get; set; }

        [JsonProperty("private_metadata")]
        public string PrivateMetadata { get; set; }
    }

    public class State
    {
        public Dictionary<string, Dictionary<string, Data>> values { get; set; }
    }

    public class Data
    {
        public string Value { get; set; }

        [JsonProperty("selected_option")]
        public SelectedOption SelectedOption { get; set; }

        [JsonProperty("selected_channel")]
        private string SelectedChannel { set { this.Value = value; } }
    }

    public class SelectedOption
    {
        public string Value { get; set; }
    }
}