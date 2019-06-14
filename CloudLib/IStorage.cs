using System.Collections.Generic;
using System.Threading.Tasks;
using CloudLib.Models;

namespace CloudLib
{
    public interface IStorage
    {
        Task<IEnumerable<QuestionnaireEntity>> GetQuestionnaires();
        Task<IEnumerable<QuestionnaireEntity>> GetQuestionnaires(string questionnaireId);

        Task<IEnumerable<AnswerEntity>> GetAnswers(string questionnaireId);
        Task InsertOrMerge(QuestionnaireEntity entity);
        Task InsertOrMerge(AnswerEntity entity);
        Task InsertOrMerge(string channel, string webHook);
    }
}