using Microsoft.Azure.Cosmos.Table;

namespace CloudLib.Models
{
    public class AnswerEntity : TableEntity
    {
        public string QuestionnaireId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Answerer { get; set; }

        public AnswerEntity()
        {
        }

        public AnswerEntity(string questionnaireId, string username)
        {
            RowKey = username;
            PartitionKey = questionnaireId;
        }
    }
}
