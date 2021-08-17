using System;
using System.Text.Json;
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
                _questionnaireId = value;
                RowKey = value;
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
                _channel = value;
                PartitionKey = value;
            }
        }
        public DateTime Created { get; set; }
        public string Question { get; set; }

        /// <summary>
        /// Message timestamp is used to send replies to the message thread
        /// </summary>
        public string MessageTimestamp { get; set; }

        private string _answerOptionsString;
        public string AnswerOptionsString
        {
            get { return _answerOptionsString; }
            set
            {
                _answerOptionsString = value;
                _answerOptions = JsonSerializer.Deserialize<string[]>(value);
            }
        }

        private string[] _answerOptions;
        public string[] AnswerOptions
        {
            get { return _answerOptions; }
            set
            {
                _answerOptions = value;
                _answerOptionsString = JsonSerializer.Serialize(_answerOptions);
            }
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
