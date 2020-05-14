using System;
using System.Threading.Tasks;
using SlackLib.Messages;

namespace AskBotCore
{
    public interface IAskBotControl
    {
        Task CreateQuestionnaire(Questionnaire questionnaire, string channel, DateTime time);
        Task<QuestionnaireResult> GetAnswers(string questionnaireId);
        Task DeleteAll();
    }
}