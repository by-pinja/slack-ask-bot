using System;
using AskBotCore;
using CloudLib;
using Microsoft.Extensions.Logging;
using SlackLib;

namespace AzureFunctions.Payloads
{
    public class PayloadBase
    {
        protected readonly SlackClient _slackClient;
        protected readonly AskBotControl _control;
        protected readonly IStorage _storage;
        protected readonly ILogger<PayloadBase> _logger;

        public PayloadBase(SlackClient slackClient, AskBotControl control, IStorage storage, ILogger<PayloadBase> logger)
        {
            _slackClient = slackClient ?? throw new ArgumentNullException(nameof(slackClient)); ;
            _control = control ?? throw new ArgumentNullException(nameof(control));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}