using System;
using Newtonsoft.Json;

namespace SlackLib.Messages
{
    public class Questionnaire
    {
        [JsonIgnoreAttribute]
        public string QuestionId { get; set; } = Guid.NewGuid().ToString();
        public string Question { get; set; }
        public string[] AnswerOptions { get; set; }
    }
}