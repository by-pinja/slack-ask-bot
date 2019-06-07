using System;
using ConsoleTester.Models;
using ConsoleTester.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            _logger.LogTrace("Creating questionnaire");
            var storage = _serviceProvider.GetService<Storage>();
            var slackConfig = _serviceProvider.GetService<SlackConfiguration>();

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
            CreateQuestionaire(questionnaire);
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

        private void CreateQuestionaire(Questionnaire questionnaire)
        {
            var client = _serviceProvider.GetService<SlackClient>();
            var result = client.PostQuestionaire("test-channel", questionnaire).Result;
            _logger.LogInformation(result.StatusCode.ToString());
        }
    }
}