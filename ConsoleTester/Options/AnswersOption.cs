using CommandLine;

namespace ConsoleTester.Options
{
    [Verb("answers", HelpText = "Gets all answers")]
    public class AnswersOption
    {
        [Option('q', "questionnaireId", HelpText = "If given, only answers related to this questionnaire are returned.", Required = false)]
        public string QuestionnaireId { get; set; }
    }
}