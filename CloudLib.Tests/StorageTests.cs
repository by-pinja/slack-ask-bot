using System;
using System.Linq;
using System.Threading.Tasks;
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

        /*
        WHY: Deleting tables can be surprisingly slow (over 10 seconds even for almost empty table)
        and delete table with API returns before the table is properly deleted. This can lead to
        Conflict 409 if we execute `CreateIfNotExists` while the table is still being deleted behind
        the scenes and we use same name.
        */
        private readonly string _answersTable = $"mockAnswers{Guid.NewGuid()}".Replace("-", string.Empty);
        private readonly string _questionsTable = $"mockQuestions{Guid.NewGuid()}".Replace("-", string.Empty);

        [SetUp]
        public void Setup()
        {
            var storageConnectionSTring = TestContext.Parameters["StorageConnectionString"];
            if (string.IsNullOrWhiteSpace(storageConnectionSTring))
            {
                Assert.Ignore("StorageConnectionString undefined. Skipping storage tests.");
            }

            var settings = new TableStorageSettings
            {
                AnswerTable = _answersTable,
                QuestionTable = _questionsTable,
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
                var actualQuestionnaire = await _storage.GetQuestionnaireOrNull(expectedQuestionniare.QuestionnaireId);
                actualQuestionnaire.MessageTimestamp.Should().Be(expectedQuestionniare.MessageTimestamp);
                actualQuestionnaire.Question.Should().Be(expectedQuestionniare.Question);
            }
        }

        [Test]
        public async Task AnsweringFlow()
        {
            var questionnaire = new QuestionnaireEntity(Guid.NewGuid().ToString(), "mockchannel")
            {
                Question = $"Who is it?",
                AnswerOptions = new[]
                {
                    "a", "b", "c"
                },
                Created = DateTime.UtcNow,
                MessageTimestamp = $"timestamp new"
            };
            await _storage.InsertOrMerge(questionnaire);

            var answers = Enumerable.Range(0, 10).Select(i => new AnswerEntity(questionnaire.QuestionnaireId, $"user {i}")
            {
                Question = $"Who is it?",
                Answer = questionnaire.AnswerOptions[i % questionnaire.AnswerOptions.Length],
                QuestionnaireId = questionnaire.QuestionnaireId,
                Answerer = $"user {i}",
            });

            foreach (var answer in answers)
            {
                await _storage.InsertOrMerge(answer);
            }

            var actualAnswers = await _storage.GetAnswers(questionnaire.QuestionnaireId);
            actualAnswers.Count().Should().Be(answers.Count());

            foreach (var answer in answers)
            {
                var actualAnswersForAnswerer = await _storage.GetAnswers(questionnaire.QuestionnaireId, answer.Answerer);
                actualAnswersForAnswerer.Count().Should().Be(1, "There should only be one answer for each answerer");
            }

            await _storage.DeleteQuestionnaireAndAnswers(questionnaire.QuestionnaireId);

            var noAnswers = await _storage.GetAnswers(questionnaire.QuestionnaireId);
            noAnswers.Should().BeEmpty();

            var removedQuestionnaire = await _storage.GetQuestionnaireOrNull(questionnaire.QuestionnaireId);
            removedQuestionnaire.Should().BeNull();
        }

        [Test]
        public async Task GetQuestionnaire_Missing()
        {
            var result = await _storage.GetQuestionnaireOrNull("missing");

            result.Should().BeNull();
        }

        [Test]
        public async Task DeleteQuestionnaireAndAnswers_Missing()
        {
            await _storage.DeleteQuestionnaireAndAnswers("missing");
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
