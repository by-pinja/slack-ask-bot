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
        public async Task CreateMultipleQuestionniaore()
        {
            var questionnaires = Enumerable.Range(0, 10).Select(i => new QuestionnaireEntity(i.ToString(), "mockchannel")
            {
                Question = $"Who is it? {i}",
                AnswerOptions = new[]
                {
                    "a", "b", "c"
                },
                Created = DateTime.UtcNow,
                MessageTimestamp = $"timestamp new {i}"
            });

            foreach (var questionnaire in questionnaires)
            {
                await _storage.InsertOrMerge(questionnaire);
            }

            var all = await _storage.GetQuestionnaires();
            foreach (var expectedQuestionniare in questionnaires)
            {
                var savedQuestionniare = all.FirstOrDefault(q => q.QuestionnaireId == expectedQuestionniare.QuestionnaireId);
                savedQuestionniare.Should().NotBeNull();

                // Test single select also
                var actualQuestionnaire = await _storage.GetQuestionnaire(expectedQuestionniare.QuestionnaireId);
                actualQuestionnaire.MessageTimestamp.Should().Be(expectedQuestionniare.MessageTimestamp);
                actualQuestionnaire.Question.Should().Be(expectedQuestionniare.Question);
            }
        }

        [Test]
        public async Task GetQuestionnaire_Missing()
        {
            var result = await _storage.GetQuestionnaire("missing");

            result.Should().BeNull();
        }


        [TearDown]
        public async Task Teardown()
        {
            if (_util != null)
            {
                await _util.DeleteAll();
            }
        }

        [OneTimeTearDown]
        public async Task TeardownOnce()
        {
            if (_util != null)
            {
                await _util.DeleteTestTables();
            }
        }
    }
}
