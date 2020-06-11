using CommandLine;

namespace ConsoleInterface.Options
{
    [Verb("generateTemplate", HelpText = "Creates a new questionnaire template.")]
    public class GenerateQuestionnaireTemplateOption
    {
        [Option('o', "outputFileName", HelpText = "Filename of the template file that is generated.", Required = true)]
        public string FileName { get; set; }
    }
}