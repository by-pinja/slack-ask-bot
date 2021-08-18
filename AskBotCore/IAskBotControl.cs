using System.Threading.Tasks;
using CloudLib.Models;
using SlackLib.Messages;

namespace AskBotCore
{
    public interface IAskBotControl
    {
        Task CreateQuestionnaire(QuestionnaireEntity questionnaire);
        Task PostResultsToThread(string questionnaireId);
        Task<QuestionnaireResult> GetQuestionnaireResult(string questionnaireId);
        Task<string> DeleteQuestionnaireAndAnswers(string questionnaireId);
    }
}
