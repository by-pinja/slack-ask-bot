using System;
using System.Text.Json;
using Microsoft.Azure.Cosmos.Table;

namespace CloudLib.Models
{
    public class QuestionnaireEntity : TableEntity
    {
        public Guid QuestionnaireId { get; set; }
        public string Channel { get; set; }
        public DateTime Created { get; set; }
        public string Question { get; set; }

        /// <summary>
        /// ';' separated answer options
        /// </summary>
        private string _answerOptionsString;
        public string AnswerOptionsString
        {
            get { return _answerOptionsString; }
            set { _answerOptions = JsonSerializer.Deserialize<string[]>(value); }
        }

        private string[] _answerOptions;
        public string[] AnswerOptions
        {
            get { return _answerOptions; }
            set { _answerOptionsString = JsonSerializer.Serialize(_answerOptions); }
        }

        public QuestionnaireEntity()
        {
        }

        public QuestionnaireEntity(Guid questionnaireId, string channel)
        {
            RowKey = questionnaireId.ToString();
            PartitionKey = channel;
        }
    }
}