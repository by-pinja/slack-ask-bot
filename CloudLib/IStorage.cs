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
        Task InsertOrMerge(QuestionnaireEntity entity);
        Task InsertOrMerge(AnswerEntity entity);
        Task DeleteAll();
        Task DeleteQuestionnaireAndAnswers(string questionnaireId);
    }
}