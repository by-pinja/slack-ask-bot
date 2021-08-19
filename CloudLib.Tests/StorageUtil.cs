using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CloudLib.Models;
using Microsoft.Azure.Cosmos.Table;

namespace CloudLib.Tests
{
    public class StorageUtil
    {
        private readonly TableStorageSettings _settings;
        private readonly CloudTableClient _client;

        private readonly CloudTable _questionnaires;
        private readonly CloudTable _answers;

        public StorageUtil(TableStorageSettings settings)
        {
            _settings = settings;

            var storageAccount = CloudStorageAccount.Parse(_settings.ConnectionString);
            _client = storageAccount.CreateCloudTableClient();

            _questionnaires = _client.GetTableReference(_settings.QuestionTable);
            _answers = _client.GetTableReference(_settings.AnswerTable);
        }
        public async Task DeleteTestTables()
        {
            await _client.GetTableReference(_settings.QuestionTable).DeleteIfExistsAsync();
            await _client.GetTableReference(_settings.AnswerTable).DeleteIfExistsAsync();
        }

        public async Task DeleteAll()
        {
            var answers = await GetAnswers();
            var answerBatchGroups = GroupedDeletes(answers);

            foreach (var batch in answerBatchGroups)
            {
                _answers.ExecuteBatch(batch);
            }

            var questionnaires = await GetQuestionnaires();
            var questionnaireBatchGroups = GroupedDeletes(questionnaires);

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
    }
}
