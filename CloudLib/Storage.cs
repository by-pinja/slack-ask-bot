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

        private async Task<IEnumerable<AnswerEntity>> GetAnswers()
        {
            var query = new TableQuery<AnswerEntity>();
            return await _answers.ExecuteQueryAsync(query);
        }

        public async Task<QuestionnaireEntity> GetQuestionnaire(string questionnaireId)
        {
            if (string.IsNullOrWhiteSpace(questionnaireId)) throw new ArgumentException("Questionnaire id is empty", nameof(questionnaireId));

            var query = new TableQuery<QuestionnaireEntity>().Where(TableQuery.GenerateFilterCondition(nameof(QuestionnaireEntity.QuestionnaireId), QueryComparisons.Equal, questionnaireId));
            var questionnaires = await _questionnaires.ExecuteQueryAsync(query);
            if (questionnaires.Count != 1)
            {
                _logger.LogDebug("{count} questionnaires found in query for questionnaire with id {questionnaireId}", questionnaires.Count, questionnaireId);
                return null;
            }

            return questionnaires.First();
        }

        public async Task<IEnumerable<AnswerEntity>> GetAnswers(string questionnaireId)
        {
            if (string.IsNullOrWhiteSpace(questionnaireId)) throw new ArgumentException("Questionnaire id is empty.", nameof(questionnaireId));
            var query = new TableQuery<AnswerEntity>().Where(TableQuery.GenerateFilterCondition(nameof(AnswerEntity.QuestionnaireId), QueryComparisons.Equal, questionnaireId));
            return await _answers.ExecuteQueryAsync(query);
        }

        public async Task InsertOrMerge(QuestionnaireEntity entity)
        {
            var insertOperation = TableOperation.InsertOrMerge(entity);
            try
            {
                await _questionnaires.ExecuteAsync(insertOperation);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Failed to execute insert or merge function. Partition key: {Pkey}, row key: {RKey}. Message: {message}", entity.PartitionKey, entity.RowKey, e.Message);
                throw;
            }
        }

        public async Task InsertOrMerge(AnswerEntity entity)
        {
            var insertOperation = TableOperation.InsertOrMerge(entity);
            try
            {
                await _answers.ExecuteAsync(insertOperation);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Failed to execute insert or merge function. Partition key: {Pkey}, row key: {RKey}. Message: {message}", entity.PartitionKey, entity.RowKey, e.Message);
                throw;
            }
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

        public async Task DeleteQuestionnaireAndAnswers(string questionnaireId)
        {
            if (string.IsNullOrWhiteSpace(questionnaireId)) throw new ArgumentException("Questionnaire id is empty", nameof(questionnaireId));

            var questionnaire = await GetQuestionnaire(questionnaireId);
            _logger.LogDebug("Found questionnaire to delete.");

            var questionnaireBatch = new TableBatchOperation
            {
                TableOperation.Delete(questionnaire)
            };
            await _questionnaires.ExecuteBatchAsync(questionnaireBatch);
            _logger.LogDebug("Deleted questionnaire.");

            await DeleteAnswers(questionnaireId);
        }

        private async Task DeleteAnswers(string questionnaireId)
        {
            var answers = await GetAnswers(questionnaireId);
            if (answers.Count() == 0)
            {
                return;
            }
            _logger.LogDebug("Found {count} answer item(s) to delete.", answers.Count());
            var answerBatch = new TableBatchOperation();
            foreach (var answer in answers)
            {
                answerBatch.Add(TableOperation.Delete(answer));
            }
            await _answers.ExecuteBatchAsync(answerBatch);
            _logger.LogDebug("Deleted answer(s).");
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