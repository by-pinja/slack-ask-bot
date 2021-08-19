using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AcceptanceTests
{
    public class TestDeployment
    {
        private string _url;

        // Although it is best to avoid Setup methods, each test in this batch must have a function url defined to be able to test.
        [SetUp]
        public void Setup()
        {
            _url = TestContext.Parameters["FunctionAppUrl"];
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
            var content = new
            {
                type = "view_submission",
                user = new
                {
                    username = "pinja"
                },
                view = new
                {
                    private_metadata = "debf4628-2313-408b-9220-89fd332c6950",
                    callback_id = "open_questionnaire",
                    state = new
                    {
                        values = new
                        {
                            AnswerBlock = new
                            {
                                title = new
                                {
                                    selected_option = new
                                    {
                                        value = "nonsense"
                                    }
                                }
                            }
                        }
                    },
                    title = new
                    {
                        text = "Submit answer"
                    }
                }
            };
            var contentString = JsonConvert.SerializeObject(content);
            var payload = "payload=" + HttpUtility.UrlEncode(contentString);
            var request = new StringContent(payload);

            var result = await client.PostAsync(_url, request);

            result.EnsureSuccessStatusCode();
        }
    }
}
