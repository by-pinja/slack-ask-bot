using System;
using Microsoft.Azure.Cosmos.Table;

namespace CloudLib.Models
{
    public class AnswerEntity : TableEntity
    {
        public string QuestionnaireId { get; set; }
        public string Question { get; set; }
        public string Channel { get; set; }
        public string Answer { get; set; }
        public string Answerer { get; set; }

        public AnswerEntity()
        {

        }

        public AnswerEntity(string triggerId, string channel)
        {
            RowKey = triggerId;
            PartitionKey = channel;
        }
    }
}