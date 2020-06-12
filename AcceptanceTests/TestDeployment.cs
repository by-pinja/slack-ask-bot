using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SlackLib;

namespace AcceptanceTests
{
    public class TestDeployment
    {
        private string _url;

        // Although it is best to avoid Setup methods, each test in this batch must have a function url defined to be able to test.
        [SetUp]
        public void Setup()
        {
            _url = "https://slackaskbot-dev-anfi.azurewebsites.net/api/askbothook?code=1kkrUmAb1EQmmHj41nwbocnShrQm589kaDcq8ed1xWZ46p5oevuNYg==";// TestContext.Parameters["FunctionAppUrl"];
            if (string.IsNullOrWhiteSpace(_url))
            {
                Assert.Ignore("Function app url undefined. Skipping acceptance test.");
            }
        }

        [Test]
        public async Task BadPayloadBadRequest()
        {
            var client = new HttpClient();
            var payload = "payload=%7B%22type%22%3A%22shortcut%22%2C%22token%22%3A%22CdL3TJ2ILNpsbQ1hWQWzajg9%22%2C%22action_ts%22%3A%221591962739.923294%22%2C%22team%22%3A%7B%22id%22%3A%22T077KUF1P%22%2C%22domain%22%3A%22pinja%22%7D%2C%22user%22%3A%7B%22id%22%3A%22UGRDKANGP%22%2C%22username%22%3A%22andrew.field%22%2C%22team_id%22%3A%22T077KUF1P%22%7D%2C%22callback_id%22%3A%22get_answers%22%2C%22trigger_id%22%3A%221204571723296.7257967057.4cdc4ff541ce12c3e104c2f2745af804%22%7D";
            // var content = new StringContent(payload, Encoding.UTF8, "text/html");
            var content = new StringContent(payload);
            var result = await client.PostAsync(_url, content);
            //AsyncTestDelegate actual = async () => await client.PostAsync(_url, content);
        }
    }
}