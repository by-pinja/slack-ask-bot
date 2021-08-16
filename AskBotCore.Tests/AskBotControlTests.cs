using System;
using System.Threading.Tasks;
using CloudLib;
using CloudLib.Models;
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
    }
}
