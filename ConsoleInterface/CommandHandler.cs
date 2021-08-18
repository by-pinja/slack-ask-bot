using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AskBotCore;
using CloudLib;
using CloudLib.Models;
using ConsoleInterface.Options;
using CsvHelper;
using Microsoft.Extensions.Logging;

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

        public async Task HandleCreateQuestionnaire(CreateQuestionnaireOption option, DateTime dateTime)
        {
            try
            {
                _logger.LogTrace("Creating questionnaire from file {file}", option.QuestionnaireFile);

                var json = await File.ReadAllTextAsync(option.QuestionnaireFile);
                var questionnaire = JsonSerializer.Deserialize<QuestionnaireEntity>(json);
                _logger.LogDebug("Questionnaire deserialized, question {0}", questionnaire.Question);
                questionnaire.Created = dateTime;

                await _control.CreateQuestionnaire(questionnaire).ConfigureAwait(false);
                _logger.LogInformation("Questionnaire created from file {0}.", option.QuestionnaireFile);
            }
            catch (IOException exception)
            {
                _logger.LogDebug(exception, "IOException encountered while trying to create questionnaire from file {file}", option.QuestionnaireFile);
                _logger.LogCritical(exception, "Unable to read file {0}. Possible reasons: File doesn't exists, file name is in invalid format, required permissions ar missing.  Unable to create questionnaire. Aborting...", option.QuestionnaireFile);
            }
            catch (JsonException exception)
            {
                _logger.LogDebug(exception, "JsonException encountered while trying to create questionnaire from file {file}", option.QuestionnaireFile);
                _logger.LogCritical(exception, "Unable to parse questionnaire from file {file}. Please make sure that file contains correct JSON. To be sure that file can be parsed, use the template generated by this program. Unable to create questionnaire. Abortting...", option.QuestionnaireFile);
            }
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

        public async Task HandleGenerateTemplate(GenerateQuestionnaireTemplateOption option, string guid)
        {
            _logger.LogTrace("Generating a questionnaire template.");

            var example = new QuestionnaireEntity(guid, "This is the channel where questionnaire will be sent to.")
            {
                Question = "What is the question?",
                AnswerOptions = new string[]
                {
                    "Option 1",
                    "Option 2",
                    "Option 3",
                    "Option 4"
                }
            };
            var json = JsonSerializer.Serialize(example, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
            await File.WriteAllTextAsync(option.FileName, json);
            _logger.LogInformation("Questionnaire template file '{file}' created.", option.FileName);
        }
    }
}
