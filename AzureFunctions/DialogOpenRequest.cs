namespace AzureFunctions
{
    public class DialogOpenRequest
    {
        public string Id { get; }
        public string QuestionnaireId { get; }
        public string Channel { get; }
        public string Answerer { get; }
        public string ResponseUrl { get; }

        public DialogOpenRequest(string id, string questionnaireId, string channel, string answerer, string responseUrl)
        {
            Id = id ?? throw new System.ArgumentNullException(nameof(id));
            Channel = channel ?? throw new System.ArgumentNullException(nameof(channel));
            QuestionnaireId = questionnaireId ?? throw new System.ArgumentNullException(nameof(questionnaireId));
            Answerer = answerer ?? throw new System.ArgumentNullException(nameof(answerer));
            ResponseUrl = responseUrl ?? throw new System.ArgumentNullException(nameof(responseUrl));
        }
    }
}