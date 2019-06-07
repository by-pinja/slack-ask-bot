using CommandLine;

namespace ConsoleTester.Options
{
    [Verb("create", HelpText = "Creates a new questionnaire")]
    public class CreateQuestionnaireOption
    {
        [Option('f', "fileName", HelpText = "File that contains questionnaire that will be created. For example file, use generateTemplate -option.", Required = true)]
        public string QuestionnaireFile { get; set; }
    }
}