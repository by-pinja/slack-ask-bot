using NUnit.Framework;
using AzureFunctions;
using System;
using Microsoft.Extensions.Logging;

namespace Tests
{
    public class PayloadParserTests
    {
        private PayloadParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new PayloadParser(new MockLogger());
        }

        [Test]
        public void Parse_ExampleContentBlockActionsParsing()
        {
            const string realLifeExample = 
                "payload=%7B%22type%22%3A%22block_actions%22%2C%22team%22%3A%7B%22id%22%3A%22" + 
                "T077KUF1P%22%2C%22domain%22%3A%22protacon%22%7D%2C%22user%22%3A%7B%22id%22%3" + 
                "A%22U0ADXFBJ7%22%2C%22username%22%3A%22heikki-jussi.niemi%22%2C%22name%22%3A" + 
                "%22heikki-jussi.niemi%22%2C%22team_id%22%3A%22T077KUF1P%22%7D%2C%22api_app_i" + 
                "d%22%3A%22AE1FBE91T%22%2C%22token%22%3A%22g9HydNIlGRqe8x3GWXOioSsf%22%2C%22c" +
                "ontainer%22%3A%7B%22type%22%3A%22message%22%2C%22message_ts%22%3A%2215604920" + 
                "31.001100%22%2C%22channel_id%22%3A%22CE2B0K7D5%22%2C%22is_ephemeral%22%3Afal" + 
                "se%7D%2C%22trigger_id%22%3A%22664748916452.7257967057.8577a04c0c2536f3d6adf0" + 
                "06ce280d05%22%2C%22channel%22%3A%7B%22id%22%3A%22CE2B0K7D5%22%2C%22name%22%3" + 
                "A%22hjni-testi%22%7D%2C%22message%22%3A%7B%22type%22%3A%22message%22%2C%22su" +
                "btype%22%3A%22bot_message%22%2C%22text%22%3A%22This+content+can%27t+be+displ" +
                "ayed.%22%2C%22ts%22%3A%221560492031.001100%22%2C%22bot_id%22%3A%22BGAHNKXFC%" +
                "22%2C%22blocks%22%3A%5B%7B%22type%22%3A%22section%22%2C%22block_id%22%3A%22c" + 
                "f3fd7ba-1223-47f1-93c7-a9701ef5567b%22%2C%22text%22%3A%7B%22type%22%3A%22mrk" + 
                "dwn%22%2C%22text%22%3A%22Onko+hyv%5Cu00e4+fiilis+VOL+666%3F%22%2C%22verbatim" + 
                "%22%3Afalse%7D%7D%2C%7B%22type%22%3A%22actions%22%2C%22block_id%22%3A%22jF2s" +
                "%22%2C%22elements%22%3A%5B%7B%22type%22%3A%22button%22%2C%22action_id%22%3A%" + 
                "22CaS0%22%2C%22text%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22" + 
                "Joo%22%2C%22emoji%22%3Atrue%7D%7D%2C%7B%22type%22%3A%22button%22%2C%22action" + 
                "_id%22%3A%22dnBg%22%2C%22text%22%3A%7B%22type%22%3A%22plain_text%22%2C%22tex" +
                "t%22%3A%22Ei%22%2C%22emoji%22%3Atrue%7D%7D%2C%7B%22type%22%3A%22button%22%2C" +
                "%22action_id%22%3A%22jc%3D%5C%2F6%22%2C%22text%22%3A%7B%22type%22%3A%22plain" +
                "_text%22%2C%22text%22%3A%22%3Afeelsbadman%3A%22%2C%22emoji%22%3Atrue%7D%7D%5" + 
                "D%7D%5D%7D%2C%22response_url%22%3A%22https%3A%5C%2F%5C%2Fhooks.slack.com%5C%" + 
                "2Factions%5C%2FT077KUF1P%5C%2F665229938293%5C%2FDFmg8d8ErDdMvRj7PU40JC6z%22%" + 
                "2C%22actions%22%3A%5B%7B%22action_id%22%3A%22CaS0%22%2C%22block_id%22%3A%22j" + 
                "F2s%22%2C%22text%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Joo" + 
                "%22%2C%22emoji%22%3Atrue%7D%2C%22type%22%3A%22button%22%2C%22action_ts%22%3A" + 
                "%221560504835.208925%22%7D%5D%7D";
            
            var result = _parser.Parse(realLifeExample) as DialogOpenRequest;
            Assert.NotNull(result);
            Assert.AreEqual("664748916452.7257967057.8577a04c0c2536f3d6adf006ce280d05", result.Id);
            Assert.AreEqual("cf3fd7ba-1223-47f1-93c7-a9701ef5567b", result.QuestionnaireId);
            Assert.AreEqual("hjni-testi", result.Channel);
            Assert.AreEqual("heikki-jussi.niemi", result.Answerer);
            Assert.AreEqual("Joo", result.Answer);
            Assert.AreEqual("https://hooks.slack.com/actions/T077KUF1P/665229938293/DFmg8d8ErDdMvRj7PU40JC6z", result.ResponseUrl);
        }

        [Test]
        public void Parse_ThrowsArgumentExceptionIfPayloadIsMissing()
        {
            const string noPayload = "load=onon";
            var exception = Assert.Throws<ArgumentException>(() => _parser.Parse(noPayload));
            Assert.AreEqual("No payload-element found in content", exception.Message);
        }

        [Test]
        public void Parse_ExampleContent()
        {
            const string realLifeExample = 
                "payload=%7B%22type%22%3A%22dialog_submission%22%2C%22submission%22%3A%7B%22answer%22%3" +
                "A%22Sigourney%20Dreamweaver%22%2C%22email%22%3A%22sigdre%40example.com%22%2C%22phone" + 
                "%22%3A%22%2B1%20800-555-1212%22%2C%22meal%22%3A%22burrito%22%2C%22comment%22%3A%22No" + 
                "%20sour%20cream%20please%22%2C%22team_channel%22%3A%22C0LFFBKPB%22%2C%22who_should_s" + 
                "ing%22%3A%22U0MJRG1AL%22%7D%2C%22callback_id%22%3A%22employee_offsite_1138b%22%2C%22" + 
                "state%22%3A%22vegetarian%22%2C%22team%22%3A%7B%22id%22%3A%22T1ABCD2E12%22%2C%22domai" +
                "n%22%3A%22coverbands%22%7D%2C%22user%22%3A%7B%22id%22%3A%22W12A3BCDEF%22%2C%22name%2" +
                "2%3A%22dreamweaver%22%7D%2C%22channel%22%3A%7B%22id%22%3A%22C1AB2C3DE%22%2C%22name%2" + 
                "2%3A%22coverthon-1999%22%7D%2C%22action_ts%22%3A%22936893340.702759%22%2C%22token%22" +
                "%3A%22M1AqUUw3FqayAbqNtsGMch72%22%2C%22response_url%22%3A%22https%3A%2F%2Fhooks.slac" + 
                "k.com%2Fapp%2FT012AB0A1%2F123456789%2FJpmK0yzoZDeRiqfeduTBYXWQ%22%7D";
            
            var result = _parser.Parse(realLifeExample) as AnswerContext;
            Assert.NotNull(result);
            Assert.AreEqual("936893340.702759", result.Id);
            Assert.AreEqual("employee_offsite_1138b", result.QuestionnaireId);
            Assert.AreEqual("coverthon-1999", result.Channel);
            Assert.AreEqual("dreamweaver", result.Answerer);
            Assert.AreEqual("Sigourney Dreamweaver", result.Answer);
            Assert.AreEqual("https://hooks.slack.com/app/T012AB0A1/123456789/JpmK0yzoZDeRiqfeduTBYXWQ", result.ResponseUrl);
        }

        private class MockLogger : ILogger
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                throw new NotImplementedException();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
            }
        }
    }
}