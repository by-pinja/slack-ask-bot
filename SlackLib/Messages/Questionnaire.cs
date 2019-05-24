namespace SlackLib.Messages
{
    public class Questionnaire
    {
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public string[] Answers { get; set; }
    }
}