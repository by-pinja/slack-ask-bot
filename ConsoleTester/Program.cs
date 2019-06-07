using System;
using System.Linq;
using CommandLine;
using ConsoleTester.Models;
using ConsoleTester.Options;
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
            using (var di = BuildDependencyInjection())
            {
                var logger = di.GetService<ILogger<Program>>();
                logger.LogTrace("Logger created, starting test run.");

                Parser.Default.ParseArguments<QuestionnairesOption, CreateQuestionnaireOption, AnswersOption>(args)
                    .WithParsed<QuestionnairesOption>(opts => {
                        logger.LogTrace("Getting all questionaires");
                        var storage = di.GetService<Storage>();
                        var result = storage.GetQuestionnaires();
                        foreach (var questionaire in result)
                        {
                            logger.LogInformation("- {0} {1} {2}", questionaire.QuestionaireId, questionaire.Question, questionaire.Created);
                        }
                    })
                    .WithParsed<CreateQuestionnaireOption>(opts => {
                        logger.LogTrace("Creating questionnaire");
                        var storage = di.GetService<Storage>();
                        var slackConfig = di.GetService<SlackConfiguration>();

                        var questionnaire = new Questionnaire
                        {
                            Question = "Mitenkäs hurisee?",
                            AnswerOptions = new string []
                            {
                                "Hyvin menee",
                                "Ei se mene",
                                ":feelsbadman:"
                            }
                        };

                        var questionnaireDto = new QuestionnaireEntity(questionnaire.QuestionId, "hjni-testi")
                        {
                            QuestionaireId = questionnaire.QuestionId,
                            Channel = "hjni-testi",
                            Created = DateTime.UtcNow,
                            Question = questionnaire.Question
                        };
                        storage.InsertOrMerge(questionnaireDto);
                        CreateQuestionaire(slackConfig, logger, questionnaire);
                    })
                    .WithParsed<AnswersOption>(opts => {
                        logger.LogTrace("Getting all answers");
                        var storage = di.GetService<Storage>();
                        var result = storage.GetAnswers();
                        foreach (var answer in result)
                        {
                            logger.LogInformation("- {0} {1} {2} {3}", answer.QuestionnaireId, answer.Answer, answer.Timestamp, answer.Answerer);
                        }
                    })
                    .WithNotParsed(errs => {
                        Console.WriteLine("I'm error");
                    });
            }
        }

        private static CloudTable PrepareQuestionaireTableForUse(ILogger<Program> logger, TableStorageSettings tableStorageSettings)
        {
            var storageAccount = CloudStorageAccount.Parse(tableStorageSettings.ConnectionString);
            var client = storageAccount.CreateCloudTableClient();
            var questionaireTable = client.GetTableReference(tableStorageSettings.QuestionTable);
            if (questionaireTable.CreateIfNotExists())
            {
                logger.LogTrace("Table {0} doesn't exist, created.", tableStorageSettings.QuestionTable);
            }
            return questionaireTable;
        }

        private static void CreateQuestionaire(SlackConfiguration config, ILogger<Program> logger, Questionnaire questionnaire)
        {
            var client = new SlackClient(config);
            var result = client.PostQuestionaire("test-channel", questionnaire).Result;
            logger.LogInformation(result.StatusCode.ToString());
        }

        private static ServiceProvider BuildDependencyInjection()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var tableStorageSettings = config.GetSection("TableStorage").Get<TableStorageSettings>();
            var slackConfiguration = config.GetSection("Slack").Get<SlackConfiguration>();
            return new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                })
                .AddSingleton<TableStorageSettings>(tableStorageSettings)
                .AddSingleton<SlackConfiguration>(slackConfiguration)
                .AddSingleton<Storage>()
                .BuildServiceProvider();
        }
    }
}
