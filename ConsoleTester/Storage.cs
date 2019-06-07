using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleTester.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace ConsoleTester
{
    public class Storage
    {
        private readonly ILogger<Storage> _logger;
        private readonly TableStorageSettings _settings;

        private readonly CloudTable _questionaires;
        private readonly CloudTable _answers;

        public Storage(ILogger<Storage> logger, TableStorageSettings settings) 
        {
            _logger = logger;
            _settings = settings;

            var storageAccount = CloudStorageAccount.Parse(_settings.ConnectionString);
            var client = storageAccount.CreateCloudTableClient();
            _questionaires = client.GetTableReference(_settings.QuestionTable);
            if (_questionaires.CreateIfNotExists())
            {
                _logger.LogTrace("Table {0} doesn't exist, created.", _settings.QuestionTable);
            }
            _answers = client.GetTableReference(_settings.AnswerTable);
            if (_answers.CreateIfNotExists())
            {
                _logger.LogTrace("Table {0} doesn't exist, created.", _settings.AnswerTable);
            }
        }

        public async Task<IEnumerable<QuestionnaireEntity>> GetQuestionnaires()
        {
            TableQuery<QuestionnaireEntity> query = new TableQuery<QuestionnaireEntity>();
            return await _questionaires.ExecuteQueryAsync(query);
        }

        public async Task<IEnumerable<AnswerEntity>> GetAnswers()
        {
            TableQuery<AnswerEntity> query = new TableQuery<AnswerEntity>();
            return await _answers.ExecuteQueryAsync(query);
        }

        public async Task InsertOrMerge(QuestionnaireEntity entity)
        {
            var insertOperation = TableOperation.InsertOrMerge(entity);
            await _questionaires.ExecuteAsync(insertOperation);
        }

        public async Task DeleteAll()
        {
            _logger.LogTrace("Clearing table {0}", _answers.Name);
            var answers = await GetAnswers();
            _logger.LogDebug("Found {0} items to delete.", answers.Count());
            var answerBatch = new TableBatchOperation();
            foreach (var answer in answers)
            {
                answerBatch.Add(TableOperation.Delete(answer));
            }
            _answers.ExecuteBatch(answerBatch);

            _logger.LogTrace("Clearing table {0}", _questionaires.Name);
            var questionnaires = await GetQuestionnaires();
            _logger.LogDebug("Found {0} items to delete.", questionnaires.Count());
            var questionnaireBatch = new TableBatchOperation();
            foreach (var quoestionnaire in questionnaires)
            {
                questionnaireBatch.Add(TableOperation.Delete(quoestionnaire));
            }
            _questionaires.ExecuteBatch(questionnaireBatch);
        }
    }
}