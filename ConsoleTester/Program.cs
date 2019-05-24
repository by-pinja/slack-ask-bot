﻿using System;
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
                            logger.LogInformation("- {0} {1}", questionaire.QuestionaireId, questionaire.Created);
                        }
                    })
                    .WithParsed<CreateQuestionnaireOption>(opts => {
                        logger.LogTrace("Creating questionnaire");
                        var storage = di.GetService<Storage>();
                        var questionnaire = new QuestionnaireEntity("QuestionId!", "hjni-testi")
                        {
                            QuestionaireId = "QuestionId!",
                            Channel = "hjni-testi",
                            Created = DateTime.UtcNow,
                        };
                        storage.InsertOrMerge(questionnaire);
                        //CreateQuestionaire(config, logger);
                    })
                    .WithParsed<AnswersOption>(opts => {
                        logger.LogTrace("Getting all answers");
                        var storage = di.GetService<Storage>();
                        var result = storage.GetAnswers();
                        foreach (var questionaire in result)
                        {
                            logger.LogInformation("- {0} {1}", questionaire.QuestionnaireId, questionaire.Answer);
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

        private static void CreateQuestionaire(SlackConfiguration config, ILogger<Program> logger)
        {
            var questionnaire = new Questionnaire
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
