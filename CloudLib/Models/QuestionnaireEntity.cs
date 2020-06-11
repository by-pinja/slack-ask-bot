using System;
using Microsoft.Azure.Cosmos.Table;

namespace CloudLib.Models
{
    public class QuestionnaireEntity : TableEntity
    {
        private string _questionnaireId;
        public string QuestionnaireId
        {
            get
            {
                return _questionnaireId;
            }
            set
            {
                RowKey = value;
                _questionnaireId = value;
            }
        }
        private string _channel;
        public string Channel
        {
            get
            {
                return _channel;
            }
            set
            {
                PartitionKey = value;
                _channel = value;
            }
        }
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
            if (string.IsNullOrWhiteSpace(questionnaireId)) throw new ArgumentException("questionnaire id is empty", nameof(questionnaireId));
            if (string.IsNullOrWhiteSpace(channel)) throw new ArgumentException("channel is empty", nameof(questionnaireId));

            QuestionnaireId = questionnaireId;
            Channel = channel;
        }
    }
}