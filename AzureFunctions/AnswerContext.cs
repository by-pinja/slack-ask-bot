namespace AzureFunctions
{
    public class AnswerContext
    {
        public string Id { get; }
        public string QuestionnaireId { get; }
        public string Channel { get; }
        public string Answerer { get; }
        public string Answer { get; }
        public string ResponseUrl { get; }

        public AnswerContext(string id, string questionnaireId, string channel, string answerer, string answer, string responseUrl)
        {
            Channel = channel ?? throw new System.ArgumentNullException(nameof(channel));
            QuestionnaireId = questionnaireId ?? throw new System.ArgumentNullException(nameof(questionnaireId));
            Id = id ?? throw new System.ArgumentNullException(nameof(id));
            Answerer = answerer ?? throw new System.ArgumentNullException(nameof(answerer));
            Answer = answer ?? throw new System.ArgumentNullException(nameof(answer));
            ResponseUrl = responseUrl ?? throw new System.ArgumentNullException(nameof(responseUrl));
        }
    }
}