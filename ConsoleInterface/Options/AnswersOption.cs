using CommandLine;

namespace ConsoleInterface.Options
{
    [Verb("answers", HelpText = "Gets all answers")]
    public class AnswersOption
    {
        [Option('q', "questionnaireId", HelpText = "If given, only answers related to this questionnaire are returned.", Required = false)]
        public string QuestionnaireId { get; set; }

        [Option('o', "outputCsvFile", HelpText= "If defined, results are written to this CSV file.", Required = false)]
        public string OutputCsvFile { get; set; }
    }
}