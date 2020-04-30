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

        public PayloadBase Parse(string content)
        {
            if (content is null) throw new ArgumentNullException(nameof(content));

            _logger.LogTrace("Parsing raw: {content}", content);
            var escaped = HttpUtility.ParseQueryString(content);
            var payload = escaped["payload"];
            if (payload is null)
            {
                throw new ArgumentException("No payload-element found in content");
            }

            _logger.LogDebug("Deserializing payload: {payload}", payload);
            var json = JsonConvert.DeserializeObject<PayloadBase>(payload);
            switch (json.Type)
            {
                case "dialog_submission":
                    return JsonConvert.DeserializeObject<DialogSubmission>(payload);
                case "block_actions":
                    return JsonConvert.DeserializeObject<BlockActions>(payload);
                case "shortcut":
                    return JsonConvert.DeserializeObject<Shortcut>(payload);
                case "view_submission":
                    return JsonConvert.DeserializeObject<ViewSubmission>(payload);
                default:
                    throw new NotImplementedException($"Unknown message type {json.Type}");
            }
        }
    }
}