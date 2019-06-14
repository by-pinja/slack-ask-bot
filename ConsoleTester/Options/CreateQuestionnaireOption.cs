using CommandLine;

namespace ConsoleTester.Options
{
    [Verb("create", HelpText = "Creates a new questionnaire from file.")]
    public class CreateQuestionnaireOption
    {
        [Option('f', "fileName", HelpText = "File that contains questionnaire that will be created. For example file, use generateTemplate -option.", Required = true)]
        public string QuestionnaireFile { get; set; }

        [Option('c', "channel", HelpText = "This is the channel were questionnaire is sent to.", Required = true)]
        public string Channel { get; set; }
    }
}