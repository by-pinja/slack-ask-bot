using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudLib;
using CloudLib.Models;
using Microsoft.Extensions.Logging;
using SlackLib;
using SlackLib.Messages;

namespace AskBotCore
{
    /// <summary>
    /// This class is the core operator for the questionnaire processes.
    /// </summary>
    public class AskBotControl
    {
        private readonly ILogger<AskBotControl> _logger;
        private readonly IStorage _storage;
        private readonly SlackClient _slackClient;

        public AskBotControl(ILogger<AskBotControl> logger, IStorage storage, SlackClient slackClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _slackClient = slackClient ?? throw new ArgumentNullException(nameof(slackClient));
        }

        public async Task<IEnumerable<QuestionnaireEntity>> GetQuestionnaires()
        {
            _logger.LogTrace("Getting all questionnaires.");

            var result = await _storage.GetQuestionnaires().ConfigureAwait(false);
            foreach (var questionnaire in result)
            {
                _logger.LogInformation("- {created} {channel} {questionnaireId} {question} {answers}", questionnaire.Created, questionnaire.Channel, questionnaire.QuestionnaireId, questionnaire.Question, questionnaire.AnswerOptions);
            }

            return result;
        }

        public async Task CreateQuestionnaire(Questionnaire questionnaire, string channel, DateTime time)
        {
            _logger.LogTrace("Creating questionnaire.");

            var questionnaireDto = new QuestionnaireEntity(questionnaire.QuestionId, channel)
            {
                QuestionnaireId = questionnaire.QuestionId,
                Channel = channel,
                Created = time,
                Question = questionnaire.Question,
                AnswerOptions = string.Join(";", questionnaire.AnswerOptions)
            };
            try
            {
                await _storage.InsertOrMerge(questionnaireDto).ConfigureAwait(false);
                _logger.LogTrace("Questionnaire stored.");
                await _slackClient.PostQuestionnaire(questionnaire, channel).ConfigureAwait(false);
                _logger.LogInformation("Questionnaire created.");
            }
            catch (SlackLibException exception)
            {
                _logger.LogDebug(exception, "SlackLibException encountered while trying to create questionnaire.");
                _logger.LogCritical("Unable to send message to Slack. See error response for details.");
            }
        }

        public async Task<QuestionnaireResult> GetAnswers(string questionnaireId)
        {
            _logger.LogTrace("Getting {questionnaireId} answers", string.IsNullOrWhiteSpace(questionnaireId) ? "all" : questionnaireId);

            var answers = await _storage.GetAnswers(questionnaireId);
            if (answers is null || answers.Count() == 0)
            {
                _logger.LogError("Could not find any answers.");
            }
            _logger.LogDebug("Found {count} answers", answers.Count());
            foreach (var answer in answers)
            {
                _logger.LogInformation("- {questionnaireId} {answer} {time} {answerer}", answer.QuestionnaireId, answer.Answer, answer.Timestamp, answer.Answerer);
            }

            var answersDictionary = new Dictionary<string, int>();
            foreach (var answer in answers)
            {
                if (answersDictionary.ContainsKey(answer.Answer))
                {
                    answersDictionary[answer.Answer]++;
                }
                else
                {
                    answersDictionary[answer.Answer] = 1;
                }
            }

            _logger.LogInformation("Answers retrieved.");
            var questionnaireResult = new QuestionnaireResult
            {
                Question = answers.First().Question,
                Answers = answersDictionary
            };

            return questionnaireResult;
        }

        public async Task DeleteAll()
        {
            _logger.LogTrace("Deleting all questionnaires and answers.");
            await _storage.DeleteAll().ConfigureAwait(false);
            _logger.LogInformation("All items deleted.");
        }
    }
}
