using System;
using Newtonsoft.Json;

namespace SlackLib.Messages
{
    public class Questionnaire
    {
        [JsonIgnore]
        public string QuestionId { get; set; } = Guid.NewGuid().ToString();
        public string Question { get; set; }
        public string[] AnswerOptions { get; set; }
    }
}