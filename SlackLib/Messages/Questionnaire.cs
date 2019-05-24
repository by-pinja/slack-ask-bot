using System;

namespace SlackLib.Messages
{
    public class Questionnaire
    {
        public string QuestionId { get; private set; } = Guid.NewGuid().ToString();
        public string Question { get; set; }
        public string[] AnswerOptions { get; set; }
    }
}