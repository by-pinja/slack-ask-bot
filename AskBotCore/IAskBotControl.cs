using System;
using System.Threading.Tasks;
using SlackLib.Messages;

namespace AskBotCore
{
    public interface IAskBotControl
    {
        Task CreateQuestionnaire(Questionnaire questionnaire, string channel, DateTime time);
        Task<QuestionnaireResult> GetQuestionnaireResult(string questionnaireId);
        Task DeleteAll();
    }
}