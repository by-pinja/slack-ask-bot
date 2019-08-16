using System;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlackLib.Payloads;

namespace SlackLib
{
    public class PayloadParser
    {
        private readonly ILogger _logger;

        public PayloadParser(ILogger<PayloadParser> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public object Parse(string content)
        {
            _logger.LogTrace("Parsing raw: {content}", content);
            var escaped = HttpUtility.ParseQueryString(content);
            var payload = escaped["payload"];
            if (payload == null)
            {
                throw new ArgumentException("No payload-element found in content");
            }

            var json = JsonConvert.DeserializeObject<PayloadBase>(payload);
            switch (json.Type)
            {
                case "dialog_submission":
                    return JsonConvert.DeserializeObject<DialogSubmission>(payload);
                case "block_actions":
                    return JsonConvert.DeserializeObject<BlockActions>(payload);
                default:
                    throw new NotImplementedException($"Unkown message type {json.Type}");
            }
        }
    }
}