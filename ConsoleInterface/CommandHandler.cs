using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using AskBotCore;
using CloudLib;
using CloudLib.Models;
using ConsoleInterface.Options;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConsoleInterface
{
    /// <summary>
    /// This class knows what to do with parsed commands
    /// </summary>
    public class CommandHandler
    {
        private readonly ILogger<CommandHandler> _logger;
        private readonly IAskBotControl _control;
        private readonly IStorage _storage;

        public CommandHandler(ILogger<CommandHandler> logger, IAskBotControl control, IStorage storage)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _control = control ?? throw new ArgumentNullException(nameof(control));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task HandleGetQuestionnaires(QuestionnairesOption option)
        {
            _logger.LogTrace("Get all questionnaires activated from console interface.");

            await _storage.GetQuestionnaires().ConfigureAwait(false);
        }

        public async Task HandleGetAnswers(AnswersOption option)
        {
            _logger.LogTrace("Get questionnaire answers activated from console interface.");

            var result = await _control.GetQuestionnaireResult(option.QuestionnaireId).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(option.OutputCsvFile))
            {
                using (var writer = new StreamWriter(option.OutputCsvFile))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(result.Answers);
                }
                _logger.LogInformation("Answers written to file: {file}.", option.OutputCsvFile);
            }
        }
    }
}
