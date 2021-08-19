using Newtonsoft.Json;

namespace SlackLib.Responses
{
    public class GenericReponse
    {
        [JsonProperty("ok", Required = Required.Always)]
        public bool Ok { get; set; }

        [JsonProperty("error", Required = Required.Default)]
        public string Error { get; set; }
    }
}
