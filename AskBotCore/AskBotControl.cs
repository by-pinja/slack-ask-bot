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
    public class AskBotControl : IAskBotControl
    {
        private readonly ILogger<AskBotControl> _logger;
        private readonly IStorage _storage;
        private readonly ISlackClient _slackClient;

        public AskBotControl(ILogger<AskBotControl> logger, IStorage storage, ISlackClient slackClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _slackClient = slackClient ?? throw new ArgumentNullException(nameof(slackClient));
        }

        public async Task CreateQuestionnaire(QuestionnaireEntity questionnaire)
        {
            if (questionnaire == null) throw new ArgumentNullException(nameof(questionnaire));
            if (string.IsNullOrWhiteSpace(questionnaire.Channel)) throw new ArgumentException("Channel is empty", nameof(questionnaire.Channel));
            _logger.LogTrace("Creating questionnaire: {questionnaire}. Channel: {channel}", questionnaire.Question, questionnaire.Channel);

            await _storage.InsertOrMerge(questionnaire).ConfigureAwait(false);
            _logger.LogTrace("Questionnaire stored.");
            var payload = PayloadUtility.GetQuestionnairePostPayload(questionnaire);
            await _slackClient.PostMessage(payload).ConfigureAwait(false);
            _logger.LogInformation("Questionnaire created.");
        }

        public async Task<QuestionnaireResult> GetQuestionnaireResult(string questionnaireId)
        {
            if (string.IsNullOrWhiteSpace(questionnaireId)) throw new ArgumentException("QuestionnaireId is empty", nameof(questionnaireId));

            var questionnaire = await _storage.GetQuestionnaire(questionnaireId);
            if (questionnaire is null)
            {
                _logger.LogError("Could not find questionnaire with id {questionnaireId}.", questionnaireId);
                throw new Exception("Could not find questionnaire.");
            }
            _logger.LogTrace("Getting {questionnaireId} answers.", questionnaire.Question);

            var answers = await _storage.GetAnswers(questionnaireId);
            _logger.LogDebug("Found {count} answer(s)", answers.Count());

            foreach (var answer in answers)
            {
                _logger.LogInformation("- {questionnaireId} {answer} {time} {answerer}", answer.QuestionnaireId, answer.Answer, answer.Timestamp, answer.Answerer);
            }

            var answersDictionary = new Dictionary<string, int>();
            foreach (var availableAnswer in questionnaire.AnswerOptions)//.Split(';'))
            {
                answersDictionary[availableAnswer] = 0;
            }
            foreach (var answer in answers)
            {
                answersDictionary[answer.Answer]++;
            }

            _logger.LogInformation("Answers retrieved.");
            var questionnaireResult = new QuestionnaireResult
            {
                Question = questionnaire.Question,
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

        public async Task<string> DeleteQuestionnaireAndAnswers(string questionnaireId)
        {
            if (string.IsNullOrWhiteSpace(questionnaireId)) throw new ArgumentException("QuestionnaireId", nameof(questionnaireId));

            var questionnaire = await _storage.GetQuestionnaire(questionnaireId);
            if (questionnaire is null)
            {
                _logger.LogError("Could not find questionnaire with id {questionnaireId}.", questionnaireId);
                throw new Exception("Could not find questionnaire.");
            }

            _logger.LogTrace("Deleting questionnaire and answers.");
            await _storage.DeleteQuestionnaireAndAnswers(questionnaireId);

            return questionnaire.Question;
        }
    }
}
