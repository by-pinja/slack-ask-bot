using System;
using System.Linq;
using System.Threading.Tasks;
using AskBotCore;
using CloudLib;
using CommandLine;
using ConsoleInterface.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlackLib;

namespace ConsoleInterface
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var di = BuildDependencyInjection())
            {
                var logger = di.GetService<ILogger<Program>>();
                logger.LogTrace("Logger created, starting test run.");
                var commandHandler = di.GetService<CommandHandler>();

                Run(args, commandHandler, logger).Wait();
            }
        }

        private static async Task Run(string[] args, CommandHandler commandHandler, ILogger<Program> logger)
        {
            await Parser.Default.ParseArguments<QuestionnairesOption, CreateQuestionnaireOption, AnswersOption, DeleteOption, GenerateQuestionnaireTemplateOption>(args)
                .MapResult(
                    async (QuestionnairesOption option) => { await commandHandler.HandleGetQuestionnaires(option); },
                    async (CreateQuestionnaireOption option) => { await commandHandler.HandleCreateQuestionnaire(option, DateTime.UtcNow); },
                    async (AnswersOption option) => { await commandHandler.HandleGetAnswers(option); },
                    async (DeleteOption option) => { await commandHandler.HandleDelete(option); },
                    async (GenerateQuestionnaireTemplateOption option) => { await commandHandler.HandleGenerateTemplate(option, Guid.NewGuid().ToString()); },
                    errors =>
                    {
                        if (errors.Count() == 1 &&
                           (errors.First().Tag == ErrorType.HelpRequestedError ||
                            errors.First().Tag == ErrorType.HelpVerbRequestedError ||
                            errors.First().Tag == ErrorType.NoVerbSelectedError ||
                            errors.First().Tag == ErrorType.VersionRequestedError))
                        {
                            return Task.FromResult(0);
                        }

                        logger.LogWarning("Something went wrong while parsing command(s)");
                        return Task.FromResult(0);
                    });
        }

        private static ServiceProvider BuildDependencyInjection()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var tableStorageSettings = config.GetSection("TableStorage").Get<TableStorageSettings>();
            var slackClientSettings = config.GetSection("SlackClient").Get<SlackClientSettings>();

            return new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                })
                .AddSingleton(tableStorageSettings)
                .AddSingleton(slackClientSettings)
                .AddTransient<ISlackClient, SlackClient>()
                .AddTransient<IStorage, Storage>()
                .AddTransient<IAskBotControl, AskBotControl>()
                .AddTransient<CommandHandler>()
                .BuildServiceProvider();
        }
    }
}
