using System;
using Microsoft.Azure.Cosmos.Table;

namespace ConsoleTester.Models
{
    public class QuestionaireEntity : TableEntity
    {
        public string QuestionaireId { get; set; }
        public string Channel { get; set; }
        public string GoogleSheetId { get; set; }

        public QuestionaireEntity()
        {

        }

        public QuestionaireEntity(string questionaireId, string channel)
        {
            RowKey = questionaireId;
            PartitionKey = channel;
        }
    }
}