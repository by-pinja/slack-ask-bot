using System.Collections.Generic;
using System.Threading.Tasks;
using CloudLib.Models;

namespace CloudLib
{
    public interface IStorage
    {
        Task<IEnumerable<QuestionnaireEntity>> GetQuestionnaires();
        Task<QuestionnaireEntity> GetQuestionnaire(string questionnaireId);
        Task<IEnumerable<AnswerEntity>> GetAnswers(string questionnaireId);
        Task<IEnumerable<AnswerEntity>> GetAnswers(string questionnaireId, string answerer);
        Task InsertOrMerge(AnswerEntity entity);
        Task InsertOrMerge(QuestionnaireEntity entity);
        Task DeleteQuestionnaireAndAnswers(string questionnaireId);
    }
}
