using System;
using System.IO;
using ConsoleTester.Models;
using ConsoleTester.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlackLib;
using SlackLib.Messages;

namespace ConsoleTester
{
    /// <summary>
    /// This class knows what to do with parsed commands
    /// </summary>
    public class CommandHandler 
    {
        private readonly ILogger<CommandHandler> _logger;
        private readonly IServiceProvider _serviceProvider;
        public CommandHandler(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetService<ILogger<CommandHandler>>();
            _serviceProvider = serviceProvider;
        }

        public void HandleGetQuestionnaires(QuestionnairesOption option)
        {
            _logger.LogTrace("Getting all questionaires");
            var storage = _serviceProvider.GetService<Storage>();
            var result = storage.GetQuestionnaires();
            foreach (var questionaire in result)
            {
                _logger.LogInformation("- {0} {1} {2}", questionaire.QuestionaireId, questionaire.Question, questionaire.Created);
            }
        }

        public void HandleCreateQuestionnaires(CreateQuestionnaireOption option)
        {
            try 
            {
                _logger.LogTrace("Creating questionnaire from file {0}", option.QuestionnaireFile);
                var storage = _serviceProvider.GetService<Storage>();
                var slackConfig = _serviceProvider.GetService<SlackConfiguration>();

                var json = File.ReadAllText(option.QuestionnaireFile);
                var questionnaire = JsonConvert.DeserializeObject<Questionnaire>(json);
                _logger.LogDebug("Questionnaire deserialized, question {0}", questionnaire.Question);

                var questionnaireDto = new QuestionnaireEntity(questionnaire.QuestionId, "hjni-testi")
                {
                    QuestionaireId = questionnaire.QuestionId,
                    Channel = "hjni-testi",
                    Created = DateTime.UtcNow,
                    Question = questionnaire.Question
                };
                storage.InsertOrMerge(questionnaireDto);
                CreateQuestionaire(questionnaire);
            }
            catch (IOException exception)
            {
                _logger.LogDebug(exception, "IOException encountered while trying to create questionnaire from file {0}", option.QuestionnaireFile);
                _logger.LogCritical("Unable to read file {0}. Possible reasons: File doesn't exists, file name is in invalid format, required permissions ar missing.  Unable to create questionnaire. Abortting...", option.QuestionnaireFile);
            }
            catch (JsonReaderException exception)
            {
                _logger.LogDebug(exception, "JsonReaderException encountered while trying to create questionnaire from file {0}", option.QuestionnaireFile);
                _logger.LogCritical("Unable to parse questionnaire from file {0}. Unable to create questionnaire. Abortting...", option.QuestionnaireFile);
            }
        }

        public void HandleGetAnswers(AnswersOption option)
        {
            _logger.LogTrace("Getting all answers");
            var storage = _serviceProvider.GetService<Storage>();
            var result = storage.GetAnswers();
            foreach (var answer in result)
            {
                _logger.LogInformation("- {0} {1} {2} {3}", answer.QuestionnaireId, answer.Answer, answer.Timestamp, answer.Answerer);
            }
        }

        public void HandleDelete(DeleteOption option)
        {
            _logger.LogTrace("Deleting all questionnaires and answers");
            var storage = _serviceProvider.GetService<Storage>();
            storage.DeleteAll();
        }

        public void HandleGenerateTemplate(GenerateQuestionnaireTemplateOption option)
        {
            _logger.LogTrace("Generating a questionnaire template");

            var example = new Questionnaire
            {
                Question = "Mitenkäs hurisee?",
                AnswerOptions = new string []
                {
                    "Hyvin menee",
                    "Ei se mene",
                    ":feelsbadman:"
                }
            };
            string json = JsonConvert.SerializeObject(example, Formatting.Indented);
            File.WriteAllText(option.FileName, json);
            _logger.LogInformation("Questionnaire template file '{0}' created.", option.FileName);
        }

        private void CreateQuestionaire(Questionnaire questionnaire)
        {
            var client = _serviceProvider.GetService<SlackClient>();
            var result = client.PostQuestionaire("test-channel", questionnaire).Result;
            _logger.LogInformation(result.StatusCode.ToString());
        }
    }
}