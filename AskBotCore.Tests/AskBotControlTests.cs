using System;
using System.Threading.Tasks;
using CloudLib;
using CloudLib.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using SlackLib;
using SlackLib.Requests;
using SlackLib.Responses;

namespace AskBotCore.Tests
{
    public class AskBotControlTests
    {
        private AskBotControl _control;

        private IStorage _mockStorage;
        private ISlackClient _mockSlackClient;

        [SetUp]
        public void Setup()
        {
            var logger = Substitute.For<ILogger<AskBotControl>>();
            _mockStorage = Substitute.For<IStorage>();
            _mockSlackClient = Substitute.For<ISlackClient>();
            _control = new AskBotControl(logger, _mockStorage, _mockSlackClient);
        }

        [Test]
        public async Task CreateQuestionnaire_InsertsQuestionnaireAndSendMessage()
        {
            var channel = "mockchannel";
            var responseChannel = "123123";
            var responseTimestamp = "timestampyes";
            var entity = new QuestionnaireEntity("id", channel);
            _mockSlackClient.PostMessage(Arg.Any<ChatPostMessageRequest>()).Returns(Task.FromResult(new ChatPostMessageResponse()
            {
                Channel = responseChannel,
                Timestamp = responseTimestamp
            }));

            await _control.CreateQuestionnaire(entity);

            await _mockStorage.Received().InsertOrMerge(entity);
            await _mockSlackClient.Received().PostMessage(Arg.Is<ChatPostMessageRequest>(cpmr => cpmr.Channel == channel));
            await _mockSlackClient.Received().ChatUpdate(Arg.Is<ChatUpdateRequest>(cur => cur.Channel == responseChannel && cur.Timestamp == responseTimestamp));
        }

        [Test]
        public async Task CreateQuestionnaire_DoesnAddQuestionnaireIfPostingFails()
        {
            var channel = "mockchannel";
            var entity = new QuestionnaireEntity("id", channel);
            _mockSlackClient.When(x => x.PostMessage(Arg.Any<ChatPostMessageRequest>())).Do(x => { throw new Exception(); });

            Assert.ThrowsAsync<Exception>(async () => await _control.CreateQuestionnaire(entity));

            await _mockStorage.DidNotReceiveWithAnyArgs().InsertOrMerge(Arg.Any<QuestionnaireEntity>());
            await _mockSlackClient.DidNotReceiveWithAnyArgs().ChatUpdate(Arg.Any<ChatUpdateRequest>());
        }

        [Test]
        public async Task DeleteQuestionnaireAndAnswers_DeletesChosenQuestionnaireAndPerformsClosing()
        {
            var entity = new QuestionnaireEntity("id", "mockchannel")
            {
                AnswerOptions = new[] { "a", "b" },
                MessageTimestamp = "messageid"
            };
            _mockStorage.GetQuestionnaireOrNull(entity.QuestionnaireId).Returns(Task.FromResult(entity));

            await _control.DeleteQuestionnaireAndAnswers(entity.QuestionnaireId);

            await _mockStorage.Received().DeleteQuestionnaireAndAnswers(entity.QuestionnaireId);
            await _mockSlackClient.Received().ChatUpdate(Arg.Is<ChatUpdateRequest>(cur => cur.Timestamp == entity.MessageTimestamp && cur.Channel == entity.Channel));
            await _mockSlackClient.Received().PostMessage(Arg.Is<ChatPostMessageRequest>(cpmr => cpmr.ThreadTimestamp == entity.MessageTimestamp && cpmr.Channel == entity.Channel));
        }

        [Test]
        public async Task GetQuestionnaireResult_ReturnsQuestionnaireResultNoAnswers()
        {
            var questionnaireId = "id";
            var questionnaire = new QuestionnaireEntity(questionnaireId, "mockchannel")
            {
                Question = "How it's going?",
                AnswerOptions = new[] { "a", "b" }
            };
            _mockStorage.GetQuestionnaireOrNull(questionnaireId).Returns(Task.FromResult(questionnaire));

            var result = await _control.GetQuestionnaireResult(questionnaireId);

            result.Question.Should().Be(questionnaire.Question, "Question should be same as in storage.");
            result.Answers.Count.Should().Be(questionnaire.AnswerOptions.Length, "All answer options should be present even without answers.");
            foreach (var expectedAnswer in questionnaire.AnswerOptions)
            {
                result.Answers[expectedAnswer].Should().Be(0, "Answer count should be 0 because there were no answers.");
            }
        }

        [Test]
        public void PostResultsToThread_ThrowsArgumentExceptionForEmtpyId()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _control.PostResultsToThread(null));
        }

        [Test]
        public void PostResultsToThread_ThrowsArgumentExceptionForMissingQuestionnaire()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _control.PostResultsToThread("not_exists"));
        }


        [Test]
        public async Task PostResultsToThread_PostsAnswersToThread()
        {
            var questionnaireId = "id";
            var questionnaire = new QuestionnaireEntity(questionnaireId, "mockchannel")
            {
                Question = "How it's going?",
                AnswerOptions = new[] { "a", "b" },
                MessageTimestamp = "messagethread.timestamp"
            };
            _mockStorage.GetQuestionnaireOrNull(questionnaireId).Returns(Task.FromResult(questionnaire));

            await _control.PostResultsToThread(questionnaireId);

            await _mockSlackClient.Received().PostMessage(Arg.Is<ChatPostMessageRequest>(cpmr => cpmr.ThreadTimestamp == questionnaire.MessageTimestamp));
        }
    }
}
