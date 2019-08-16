using System;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureFunctions
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

            JObject json = JsonConvert.DeserializeObject<JObject>(payload);
            var type = json.RequireString(x => x.type);
            switch (type)
            {
                case "dialog_submission":
                    return new AnswerContext(
                        json.RequireString(x => x.action_ts),
                        json.RequireString(x => x.callback_id),
                        json.RequireString(x => x.channel.name),
                        json.RequireString(x => x.user.name),
                        json.RequireString(x => x.submission.answer),
                        json.RequireString(x => x.response_url));
                case "block_actions":
                    return new DialogOpenRequest(
                        json.RequireString(x => x.trigger_id),
                        json.RequireString(x => x.message.blocks[0].block_id),
                        json.RequireString(x => x.channel.name),
                        json.RequireString(x => x.user.username),
                        json.RequireString(x => x.response_url));
                default:
                    throw new NotImplementedException($"Unkown message type {type}");
            }

        }
    }
}