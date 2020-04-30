namespace SlackLib.Messages
{
    public class Questionnaire
    {
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public string[] AnswerOptions { get; set; }
    }
}