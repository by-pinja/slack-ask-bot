using System;
using Microsoft.Azure.Cosmos.Table;

namespace CloudLib.Models
{
    public class QuestionnaireEntity : TableEntity
    {
        public string QuestionaireId { get; set; }
        public string Channel { get; set; }
        public DateTime Created { get; set; }
        public string Question { get; set; }

        /// <summary>
        /// ';' separated answer options
        /// </summary>
        public string AnswerOptions { get; set; }

        public QuestionnaireEntity()
        {

        }

        public QuestionnaireEntity(string questionaireId, string channel)
        {
            RowKey = questionaireId;
            PartitionKey = channel;
        }
    }
}