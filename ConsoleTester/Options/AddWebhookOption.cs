using CommandLine;

namespace ConsoleTester.Options
{
    [Verb("upsertChannel", HelpText = "This is used to add new supported channel when you have webhook url. This can also be used to change the webhook configured for the channel")]
    public class AddWebhookOption
    {
        [Option('c', "channel", HelpText = "Name of the channel", Required = true)]
        public string Channel { get; set; }

        [Option('w', "webHookUrl", HelpText= "Web Hook Url. This can be retrieved ", Required = true)]
        public string WebHookUrl { get; set; }
    }
}