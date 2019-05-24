using System;
using System.Linq;
using CommandLine;
using ConsoleTester.Models;
using Microsoft.Azure.Cosmos.Table;
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

                var tableStorageSettings = config.GetSection("TableStorage").Get<TableStorageSettings>();
                var questionaireTable = PrepareTableForUse(logger, tableStorageSettings);

                Parser.Default.ParseArguments<QuestionairesOption, CreateQuestionaireOption>(args)
                    .WithParsed<QuestionairesOption>(opts => {
                        logger.LogTrace("Getting all questionaires");
                        TableQuery<QuestionaireEntity> query = new TableQuery<QuestionaireEntity>();
                        var result = questionaireTable.ExecuteQuery(query);
                        foreach (var x in result)
                        {
                            logger.LogInformation("- {0} {1}", x.QuestionaireId, x.GoogleSheetId);
                        }
                    })
                    .WithParsed<CreateQuestionaireOption>(opts => {
                        logger.LogTrace("Creating questionaire");
                        var questionaire = new QuestionaireEntity("QuestionId!", "hjni-testi")
                        {
                            QuestionaireId = "QuestionId!",
                            Channel = "hjni-testi",
                            GoogleSheetId = "19zPGQw8iOpeEqUOGc-RFABq74cWz6Qpny3qs9lW9eNQ",
                        };
                        var insertOperation = TableOperation.InsertOrMerge(questionaire);
                        var result = questionaireTable.Execute(insertOperation);

                        logger.LogInformation("Ja laskua tuli: {0}", result.RequestCharge);
                        CreateQuestionaire(config, logger);
                    })
                    .WithNotParsed(errs => {
                        Console.WriteLine("I'm error");
                    });
            }
        }

        private static CloudTable PrepareTableForUse(ILogger<Program> logger, TableStorageSettings tableStorageSettings)
        {
            var storageAccount = CloudStorageAccount.Parse(tableStorageSettings.ConnectionString);
            var client = storageAccount.CreateCloudTableClient();
            
            var questionaireTable = client.GetTableReference(tableStorageSettings.Table);
            
            if (questionaireTable.CreateIfNotExists())
            {
                logger.LogTrace("Table {0} doesn't exist, creating.", tableStorageSettings.Table);
                questionaireTable.CreateIfNotExists();
            }
            return questionaireTable;
        }

        private static void CreateQuestionaire(IConfiguration config, ILogger<Program> logger)
        {
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
            var result = client.PostQuestionaire("test-channel", questionaire).Result;
            logger.LogInformation(result.StatusCode.ToString());
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
