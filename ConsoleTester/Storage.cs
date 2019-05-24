using System.Collections.Generic;
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

        public IEnumerable<QuestionnaireEntity> GetQuestionnaires()
        {
            TableQuery<QuestionnaireEntity> query = new TableQuery<QuestionnaireEntity>();
            return _questionaires.ExecuteQuery(query);
        }

        public IEnumerable<AnswerEntity> GetAnswers()
        {
            TableQuery<AnswerEntity> query = new TableQuery<AnswerEntity>();
            return _answers.ExecuteQuery(query);
        }

        public void InsertOrMerge(QuestionnaireEntity entity)
        {
            var insertOperation = TableOperation.InsertOrMerge(entity);
            _questionaires.Execute(insertOperation);
        }
    }
}