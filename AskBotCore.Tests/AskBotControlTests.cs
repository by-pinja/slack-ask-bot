using System.Threading.Tasks;
using CloudLib;
using CloudLib.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using SlackLib;
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
            var entity = new QuestionnaireEntity("id", "mockchannel");
            _mockSlackClient.PostMessage(Arg.Any<object>()).Returns(Task.FromResult<ChatPostMessageResponse>(new ChatPostMessageResponse()));

            await _control.CreateQuestionnaire(entity);

            await _mockStorage.Received().InsertOrMerge(entity);
            _mockSlackClient.Received().PostMessage(Arg.Any<dynamic>());
        }
    }
}
