using System;
using NUnit.Framework;
using SlackLib;

namespace SlackLibTests
{
    public class SlackResponseParserTests
    {
        private SlackResponseParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new SlackResponseParser();
        }

        [Test]
        public void Parse_ThrowsArgumentExceptionIfContentIsNull()
        {
            var exception = Assert.Throws<ArgumentException>(() => _parser.Parse(null));
            Assert.AreEqual("content", exception.ParamName);
        }

        [Test]
        public void Parse_ThrowsArgumentExceptionIfContentIsEmpty()
        {
            var exception = Assert.Throws<ArgumentException>(() => _parser.Parse(""));
            Assert.AreEqual("content", exception.ParamName);
        }

        [Test]
        public void Parse_ParsesOkCorrectly()
        {
            var responseString = "{\"ok\": true,\"message_ts\":\"1502210682.580145\"}";

            var response = _parser.Parse(responseString);
            Assert.IsTrue(response.Ok);
        }

        [Test]
        public void Parse_ParsesFalseCorrectly()
        {
            var responseString = "{\"ok\": false,\"error\":\"user_not_in_channel\"}";

            var response = _parser.Parse(responseString);
            Assert.IsFalse(response.Ok);
        }
    }
}