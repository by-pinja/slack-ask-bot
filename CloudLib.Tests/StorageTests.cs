using System;
using System.Linq;
using System.Threading.Tasks;
using CloudLib;
using CloudLib.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace CloudLib.Tests
{
    /// <summary>
    /// Integration tests for storage. There require actual storage existing.
    /// </summary>
    public class StorageTests
    {
        private Storage _storage;
        private StorageUtil _util;

        [SetUp]
        public void Setup()
        {
            var storageConnectionSTring = TestContext.Parameters["StorageConnectionString"];
            if (string.IsNullOrWhiteSpace(storageConnectionSTring))
            {
                Assert.Ignore("StorageConnectionString undefined. Skipping storage tests.");
            }

            var timestamp = DateTime.UtcNow.Millisecond.ToString();
            var settings = new TableStorageSettings
            {
                AnswerTable = $"mockAnswers{timestamp}",
                QuestionTable = $"mockQuestions{timestamp}",
                ConnectionString = storageConnectionSTring
            };

            _util = new StorageUtil(settings);

            var logger = Substitute.For<ILogger<Storage>>();
            _storage = new Storage(logger, settings);
        }

        [Test]
        public async Task BasicCreate()
        {
            var id = Guid.NewGuid().ToString();
            var questionnaire = new QuestionnaireEntity(id, "mockchannel")
            {
                Question = "Who is it?",
                AnswerOptions = new[]
                {
                    "a", "b", "c"
                },
                Created = DateTime.UtcNow,
                MessageTimestamp = "timestamp new"
            };

            await _storage.InsertOrMerge(questionnaire);

            var all = await _storage.GetQuestionnaires();
            var savedQuestionniare = all.FirstOrDefault(q => q.QuestionnaireId == id);
            savedQuestionniare.Should().NotBeNull();
        }

        [TearDown]
        public async Task Teardown()
        {
            await _util.DeleteAll();
        }

        [OneTimeTearDown]
        public async Task TeardownOnce()
        {
            await _util.DeleteTestTables();
        }
    }
}
