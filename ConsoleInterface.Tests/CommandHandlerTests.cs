using System.Threading.Tasks;
using AskBotCore;
using CloudLib;
using ConsoleInterface.Options;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using SlackLib.Messages;

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

        [Test]
        public async Task HandleGetAnswers_ReturnsAnswersFromAskBotControl()
        {
            var expectedResult = new QuestionnaireResult();
            var option = new AnswersOption()
            {
                QuestionnaireId = "id",
                OutputCsvFile = null
            };
            _mockAskBotControl.GetQuestionnaireResult(option.QuestionnaireId).Returns(Task.FromResult(expectedResult));

            await _handler.HandleGetAnswers(option);

            await _mockAskBotControl.Received().GetQuestionnaireResult(option.QuestionnaireId);
        }
    }
}
