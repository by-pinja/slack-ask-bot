using System.Threading.Tasks;
using CloudLib.Models;
using SlackLib.Messages;

namespace AskBotCore
{
    public interface IAskBotControl
    {
        Task CreateQuestionnaire(QuestionnaireEntity questionnaire);
        Task<QuestionnaireResult> GetQuestionnaireResult(string questionnaireId);
        Task DeleteAll();
        Task<string> DeleteQuestionnaireAndAnswers(string questionnaireId);
    }
}