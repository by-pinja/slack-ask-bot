using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SlackLib.Requests;

namespace SlackLib.Tests
{
    public class SlackClientTests
    {
        [Test]
        public void PostMessage_CorrectPayloadGivesNoError()
        {
            var logger = new MockLogger<SlackClient>();
            var httpClient = GetMockHttpClient(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent("{\"ok\":true}") });
            var slackClient = new SlackClient(logger, httpClient);

            var payload = new ChatPostMessageRequest
            {
                Channel = "channel",
                Text = "Dummy"
            };

            AsyncTestDelegate actual = async () => await slackClient.PostMessage(payload);
            Assert.DoesNotThrowAsync(actual);
            Assert.AreEqual(logger.LastLogLevel, LogLevel.Debug);
        }

        [Test]
        public void PostMessage_SlackAPIErrorThrowsSlackError()
        {
            var logger = new MockLogger<SlackClient>();
            var httpClient = GetMockHttpClient(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent("{\"ok\":false, \"error\":\"slack error\"}") });
            var slackClient = new SlackClient(logger, httpClient);

            var payload = new ChatPostMessageRequest
            {
                Channel = "channel",
                Text = "Dummy"
            };

            AsyncTestDelegate actual = async () => await slackClient.PostMessage(payload);
            Assert.ThrowsAsync<SlackLibException>(actual);
            Assert.AreEqual(logger.LastLogLevel, LogLevel.Critical);
        }

        [Test]
        public void PostMessage_BadResponseThrowsError()
        {
            var logger = new MockLogger<SlackClient>();
            var httpClient = GetMockHttpClient(new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent("{\"ok\":true}") });
            var slackClient = new SlackClient(logger, httpClient);

            var payload = new ChatPostMessageRequest
            {
                Channel = "channel",
                Text = "Dummy"
            };

            AsyncTestDelegate actual = async () => await slackClient.PostMessage(payload);
            Assert.ThrowsAsync<SlackLibException>(actual);
            Assert.AreEqual(logger.LastLogLevel, LogLevel.Critical);
        }

        private HttpClient GetMockHttpClient(HttpResponseMessage response)
        {
            var handle = new HttpMessageHandle(response);
            return new HttpClient(handle) { BaseAddress = new Uri("https://slack.com/api/") };
        }
    }
}
