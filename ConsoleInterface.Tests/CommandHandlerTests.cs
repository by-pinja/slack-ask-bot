using System.Threading.Tasks;
using AskBotCore;
using CloudLib;
using ConsoleInterface.Options;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace ConsoleInterface
{
    public class CommandHandlerTests
    {
        private CommandHandler _handler;

        private IAskBotControl _mockAskBotControl;
        private IStorage _mockStorage;

        [SetUp]
        public void Setup()
        {
            _mockAskBotControl = Substitute.For<IAskBotControl>();
            _mockStorage = Substitute.For<IStorage>();

            var logger = Substitute.For<ILogger<CommandHandler>>();
            _handler = new CommandHandler(logger, _mockAskBotControl, _mockStorage);
        }

        [Test]
        public async Task HandleGetQuestionnaires_ReturnsQuestionsFromStorage()
        {
            var option = new QuestionnairesOption();

            await _handler.HandleGetQuestionnaires(option);

            await _mockStorage.Received().GetQuestionnaires();
        }
    }
}
