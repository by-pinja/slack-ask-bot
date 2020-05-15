using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudLib.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace CloudLib
{
    public class Storage : IStorage
    {
        private readonly ILogger<Storage> _logger;
        private readonly TableStorageSettings _settings;

        private readonly CloudTable _questionnaires;
        private readonly CloudTable _answers;

        public Storage(ILogger<Storage> logger, TableStorageSettings settings)
        {
            _logger = logger;
            _settings = settings;

            var storageAccount = CloudStorageAccount.Parse(_settings.ConnectionString);
            var client = storageAccount.CreateCloudTableClient();
            _questionnaires = client.GetTableReference(_settings.QuestionTable);
            if (_questionnaires.CreateIfNotExists())
            {
                _logger.LogTrace("Table {table} doesn't exist, created.", _settings.QuestionTable);
            }
            _answers = client.GetTableReference(_settings.AnswerTable);
            if (_answers.CreateIfNotExists())
            {
                _logger.LogTrace("Table {table} doesn't exist, created.", _settings.AnswerTable);
            }
        }

        public async Task<IEnumerable<QuestionnaireEntity>> GetQuestionnaires()
        {
            var query = new TableQuery<QuestionnaireEntity>();
            return await _questionnaires.ExecuteQueryAsync(query);
        }

        public async Task<IEnumerable<AnswerEntity>> GetAnswers()
        {
            var query = new TableQuery<AnswerEntity>();
            return await _answers.ExecuteQueryAsync(query);
        }

        public async Task<QuestionnaireEntity> GetQuestionnaire(string questionnaireId)
        {
            var query = new TableQuery<QuestionnaireEntity>().Where(TableQuery.GenerateFilterCondition(nameof(QuestionnaireEntity.QuestionnaireId), QueryComparisons.Equal, questionnaireId));
            var questionnaires = await _questionnaires.ExecuteQueryAsync(query);
            if (questionnaires.Count != 1)
            {
                throw new Exception($"{questionnaires.Count} questionnaires found in query for questionnaire with id {questionnaireId} ");
            }

            return questionnaires.First();
        }

        public async Task<IEnumerable<AnswerEntity>> GetAnswers(string questionnaireId)
        {
            if (string.IsNullOrWhiteSpace(questionnaireId))
            {
                throw new Exception("Empty questionnaire Id.");
            }
            var query = new TableQuery<AnswerEntity>().Where(TableQuery.GenerateFilterCondition(nameof(AnswerEntity.QuestionnaireId), QueryComparisons.Equal, questionnaireId));
            return await _answers.ExecuteQueryAsync(query);
        }

        public async Task InsertOrMerge(QuestionnaireEntity entity)
        {
            var insertOperation = TableOperation.InsertOrMerge(entity);
            await _questionnaires.ExecuteAsync(insertOperation);
        }

        public async Task InsertOrMerge(AnswerEntity entity)
        {
            var insertOperation = TableOperation.InsertOrMerge(entity);
            await _answers.ExecuteAsync(insertOperation);
        }

        public async Task DeleteAll()
        {
            _logger.LogTrace("Clearing table {table}", _answers.Name);
            var answers = await GetAnswers();
            _logger.LogDebug("Found {count} items to delete.", answers.Count());
            var answerBatchGroups = GroupedDeletes(answers);

            _logger.LogDebug("Executing batches");
            foreach (var batch in answerBatchGroups)
            {
                _answers.ExecuteBatch(batch);
            }

            _logger.LogTrace("Clearing table {table}", _questionnaires.Name);
            var questionnaires = await GetQuestionnaires();
            _logger.LogDebug("Found {count} items to delete.", questionnaires.Count());
            var questionnaireBatchGroups = GroupedDeletes(questionnaires);

            _logger.LogDebug("Executing batch");
            foreach (var batch in questionnaireBatchGroups)
            {
                _questionnaires.ExecuteBatch(batch);
            }
        }

        private IEnumerable<TableBatchOperation> GroupedDeletes(IEnumerable<TableEntity> entities)
        {
            return entities.GroupBy(entity => entity.PartitionKey).Select(group =>
            {
                var batch = new TableBatchOperation();
                foreach (var answer in group)
                {
                    batch.Add(TableOperation.Delete(answer));
                }
                return batch;
            });
        }
    }
}