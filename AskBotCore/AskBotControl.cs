using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudLib;
using CloudLib.Models;
using Microsoft.Extensions.Logging;
using SlackLib;
using SlackLib.Messages;
using SlackLib.Requests;
using SlackLib.Responses;

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

            var preMessage = new ChatPostMessageRequest
            {
                Channel = questionnaire.Channel,
                Text = "This is a message which verifies that the bot is able to message to this channel."
            };
            ChatPostMessageResponse result = await _slackClient.PostMessage(preMessage).ConfigureAwait(false);

            questionnaire.MessageTimestamp = result.Timestamp;
            await _storage.InsertOrMerge(questionnaire).ConfigureAwait(false);
            _logger.LogTrace("Questionnaire stored.");

            var payload = PayloadUtility.GetQuestionnaireUpdatePostPayload(result.Channel, result.Timestamp, questionnaire);
            await _slackClient.ChatUpdate(payload).ConfigureAwait(false);

            _logger.LogInformation("Questionnaire created channel {channel}, ts: {timestamp}.", result.Channel, result.Timestamp);
        }

        public async Task PostResultsToThread(string questionnaireId)
        {
            var questionnaire = await GetQuestionnaireOrThrow(questionnaireId);
            var answersDictionary = await CreateResultDictionary(questionnaire);

            var preMessage = new ChatPostMessageRequest
            {
                Channel = questionnaire.Channel,
                Text = PayloadUtility.AnswersPostText(answersDictionary),
                ThreadTimestamp = questionnaire.MessageTimestamp
            };
            await _slackClient.PostMessage(preMessage).ConfigureAwait(false);
        }

        public async Task<QuestionnaireResult> GetQuestionnaireResult(string questionnaireId)
        {
            var questionnaire = await GetQuestionnaireOrThrow(questionnaireId);
            var answersDictionary = await CreateResultDictionary(questionnaire);

            _logger.LogInformation("Answers retrieved.");
            var questionnaireResult = new QuestionnaireResult
            {
                Question = questionnaire.Question,
                Answers = answersDictionary
            };

            return questionnaireResult;
        }

        private async Task<Dictionary<string, int>> CreateResultDictionary(QuestionnaireEntity questionnaire)
        {
            _logger.LogTrace("Getting {questionnaireId} answers.", questionnaire.Question);
            var answers = await _storage.GetAnswers(questionnaire.QuestionnaireId);
            _logger.LogDebug("Found {count} answer(s)", answers.Count());
            var answersDictionary = new Dictionary<string, int>();
            foreach (var availableAnswer in questionnaire.AnswerOptions)
            {
                answersDictionary[availableAnswer] = 0;
            }
            foreach (var answer in answers)
            {
                answersDictionary[answer.Answer]++;
            }

            return answersDictionary;
        }

        public async Task<string> DeleteQuestionnaireAndAnswers(string questionnaireId)
        {
            var questionnaire = await GetQuestionnaireOrThrow(questionnaireId);

            // Post final results
            await PostResultsToThread(questionnaireId);

            _logger.LogTrace("Deleting questionnaire and answers.");
            await _storage.DeleteQuestionnaireAndAnswers(questionnaireId);

            var questionnaireClosedMessage = new ChatPostMessageRequest
            {
                Channel = questionnaire.Channel,
                Text = "Questionnaire is now closed.",
                ThreadTimestamp = questionnaire.MessageTimestamp
            };
            await _slackClient.PostMessage(questionnaireClosedMessage).ConfigureAwait(false);

            var payload = PayloadUtility.GetQuestionnaireClosedPostUpdatePayload(questionnaire.Channel, questionnaire.MessageTimestamp, questionnaire);
            await _slackClient.ChatUpdate(payload).ConfigureAwait(false);

            return questionnaire.Question;
        }

        private async Task<QuestionnaireEntity> GetQuestionnaireOrThrow(string questionnaireId)
        {
            if (string.IsNullOrWhiteSpace(questionnaireId)) throw new ArgumentException("questionnaireId is empty", nameof(questionnaireId));

            var questionnaire = await _storage.GetQuestionnaireOrNull(questionnaireId);
            if (questionnaire is null)
            {
                _logger.LogError("Could not find questionnaire with id {questionnaireId}.", questionnaireId);
                throw new ArgumentException("Could not find questionnaire.", nameof(questionnaireId));
            }

            return questionnaire;
        }
    }
}
