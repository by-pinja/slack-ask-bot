using System.Collections.Generic;

namespace SlackLib.Messages
{
    public class QuestionnaireResult
    {
        public string Question { get; set; }
        public Dictionary<string, int> Answers { get; set; }
    }
}