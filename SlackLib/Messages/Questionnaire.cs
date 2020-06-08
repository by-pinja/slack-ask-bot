using System;

namespace SlackLib.Messages
{
    public class Questionnaire
    {
        public Guid QuestionId { get; set; }
        public string Question { get; set; }
        public string[] AnswerOptions { get; set; }
    }
}