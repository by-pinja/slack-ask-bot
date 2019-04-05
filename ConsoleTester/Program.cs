using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlackLib;
using SlackLib.Messages;

namespace ConsoleTester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            using (var di = BuildDependencyInjection(config))
            {
                var logger = di.GetService<ILogger<Program>>();
                logger.LogTrace("Logger created, starting test run.");

                var slackSection = config.GetSection("Slack");
                if (!slackSection.Exists())
                {
                    logger.LogError("No Slack configuration found");
                    return;
                }
                var slackConfig = new SlackConfiguration();
                slackSection.Bind(slackConfig);

                var questionaire = new Questionaire
                {
                    QuestionId = "QuestionId!",
                    Question = "Mitenkäs hurisee?",
                    Answers = new string []
                    {
                        "Hyvin menee",
                        "Ei se mene",
                        ":feelsbadman:"
                    }
                };

                var client = new SlackClient(slackConfig);
                var result = client.PostQuestionaire(questionaire).Result;
                logger.LogInformation(result.StatusCode.ToString());
            }
        }

        private static ServiceProvider BuildDependencyInjection(IConfiguration config)
        {
            return new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                }).BuildServiceProvider();
        }
    }
}
