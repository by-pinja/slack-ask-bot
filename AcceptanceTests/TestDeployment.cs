using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AcceptanceTests
{
    // This testing class has "slack-askbot-test-channel" slack channel to use for testing.
    public class TestDeployment
    {
        private string _url;

        // Although it is best to avoid Setup methods, each test in this batch must have a function url defined to be able to test.
        [SetUp]
        public void Setup()
        {
            _url = "https://slackaskbot-dev-anfi.azurewebsites.net/api/askbothook?code=1kkrUmAb1EQmmHj41nwbocnShrQm589kaDcq8ed1xWZ46p5oevuNYg=="; // TestContext.Parameters["FunctionAppUrl"];
            if (string.IsNullOrWhiteSpace(_url))
            {
                Assert.Ignore("Function app url undefined. Skipping acceptance test.");
            }
        }

        [Test]
        public async Task CorrectDummyPayload_Successful()
        {
            var client = new HttpClient();

            // Simple dummy payload for submitting an answer. The ask bot should return a JsonResult();
            var payload = "payload=%7B%22type%22%3A%22view_submission%22%2C%22user%22%3A%7B%22username%22%3A%22andrew.field%22%7D%2C%22view%22%3A%7B%22private_metadata%22%3A%22debf4628-2313-408b-9220-89fd332c6950%22%2C%22callback_id%22%3A%22open_questionnaire%22%2C%22state%22%3A%7B%22values%22%3A%7B%22AnswerBlock%22%3A%7B%22title%22%3A%7B%22selected_option%22%3A%7B%22value%22%3A%22nonsense%22%7D%7D%7D%7D%7D%2C%22title%22%3A%7B%22text%22%3A%22Submit+answer%22%7D%7D%7D";
            var content = new StringContent(payload);

            var result = await client.PostAsync(_url, content);
            result.EnsureSuccessStatusCode();
        }

        [Test]
        public async Task CorrectPayload_PostQuestionnaireToChannel_OkResult()
        {
            var client = new HttpClient();

            // http url payload to send questionnaire via post message to channel.
            var payload = "payload=%7B%22type%22%3A%22view_submission%22%2C%22user%22%3A%7B%22username%22%3A%22andrew.field%22%7D%2C%22view%22%3A%7B%22callback_id%22%3A%22create_questionnaire%22%2C%22state%22%3A%7B%22values%22%3A%7B%22TitleBlock%22%3A%7B%22title%22%3A%7B%22value%22%3A%22Automatic+deployment+test+questionnaire%22%7D%7D%2C%22ChannelBlock%22%3A%7B%22channel%22%3A%7B%22selected_channel%22%3A%22C015CJXTKL5%22%7D%7D%2C%22AnswerBlock1%22%3A%7B%22option_1%22%3A%7B%22value%22%3A%22Kyll%5Cu00e4%22%7D%7D%2C%22AnswerBlock2%22%3A%7B%22option_2%22%3A%7B%22value%22%3A%22Ei%22%7D%7D%7D%7D%7D%7D";
            var content = new StringContent(payload);

            var result = await client.PostAsync(_url, content);
            result.EnsureSuccessStatusCode();
            // You can also check slack-askbot-test-channel for the posted message.
        }
    }
}