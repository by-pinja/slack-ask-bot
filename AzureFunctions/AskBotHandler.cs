using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AskBotCore;
using AzureFunctions.Payloads;
using CloudLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlackLib;

namespace AzureFunctions
{
    public class AskBotHandler
    {
        private readonly ILogger<AskBotHandler> _logger;
        private readonly IStorage _storage;
        private readonly SlackClient _slackClient;
        private readonly AskBotControl _control;

        public AskBotHandler(ILogger<AskBotHandler> logger, IStorage storage, SlackClient slackClient, AskBotControl control)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _slackClient = slackClient ?? throw new ArgumentNullException(nameof(slackClient));
            _control = control ?? throw new ArgumentNullException(nameof(control));
        }

        [FunctionName(nameof(AskBotHook))]
        public async Task<IActionResult> AskBotHook([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req)
        {
            _logger.LogDebug("AskBot hook launched");
            var contentString = await req.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(contentString))
            {
                throw new ArgumentException("Content of HttpRequest is empty.");
            }

            _logger.LogTrace("Parsing raw: {content}", contentString);
            var escaped = HttpUtility.ParseQueryString(contentString);
            var payloadString = escaped["payload"];
            if (string.IsNullOrWhiteSpace(payloadString))
            {
                throw new ArgumentException("No payload element found in content.");
            }

            _logger.LogDebug("Deserializing payload: {payload}", payloadString);
            var json = JsonConvert.DeserializeObject<dynamic>(payloadString);

            IPayload payload;
            switch (json.Type)
            {
                case "block_actions":
                    payload = JsonConvert.DeserializeObject<BlockActions>(payloadString);
                    break;
                case "shortcut":
                    payload = JsonConvert.DeserializeObject<Shortcut>(payloadString);
                    break;
                case "view_submission":
                    payload = JsonConvert.DeserializeObject<ViewSubmission>(payloadString);
                    break;
                default:
                    throw new NotImplementedException($"Unknown payload type {json.Type}.");
            }

            return await payload.Handle();
        }
    }
}