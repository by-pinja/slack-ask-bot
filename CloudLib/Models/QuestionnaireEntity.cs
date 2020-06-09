using System;
using Microsoft.Azure.Cosmos.Table;

namespace CloudLib.Models
{
    public class QuestionnaireEntity : TableEntity
    {
        public string QuestionnaireId { get; set; }
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
            set
            {
                _answerOptionsString = value;
                _answerOptions = value.Split(';');
            }// string.Join(";", questionnaire.AnswerOptions);} //JsonSerializer.Deserialize<string[]>(value); }
        }

        private string[] _answerOptions;
        public string[] AnswerOptions
        {
            get { return _answerOptions; }
            set
            {
                _answerOptions = value;
                _answerOptionsString = string.Join(";", value);
            }//JsonSerializer.Serialize(_answerOptions); }
        }

        public QuestionnaireEntity()
        {
        }

        public QuestionnaireEntity(string questionnaireId, string channel)
        {
            RowKey = questionnaireId;
            PartitionKey = channel;
        }
    }
}