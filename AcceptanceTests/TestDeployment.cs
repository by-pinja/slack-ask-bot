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
            _url = TestContext.Parameters["FunctionAppUrl"];
            if (string.IsNullOrWhiteSpace(_url))
            {
                Assert.Ignore("Function app url undefined. Skipping acceptance test.");
            }
        }

        [Test]
        public async Task CorrectPayload_PostQuestionnaireToChannel_OkResult()
        {
            var client = new HttpClient();

            // http url payload to send questionnaire via post message to channel.
            var payload = "payload=%7B%22type%22%3A%22view_submission%22%2C%22team%22%3A%7B%22id%22%3A%22T077KUF1P%22%2C%22domain%22%3A%22pinja%22%7D%2C%22user%22%3A%7B%22id%22%3A%22UGRDKANGP%22%2C%22username%22%3A%22andrew.field%22%2C%22name%22%3A%22andrew.field%22%2C%22team_id%22%3A%22T077KUF1P%22%7D%2C%22api_app_id%22%3A%22A012L17EV33%22%2C%22token%22%3A%22CdL3TJ2ILNpsbQ1hWQWzajg9%22%2C%22trigger_id%22%3A%221182794960405.7257967057.fdd5ba08d391809d7fb077b4a55a0a51%22%2C%22view%22%3A%7B%22id%22%3A%22V015E53EWSW%22%2C%22team_id%22%3A%22T077KUF1P%22%2C%22type%22%3A%22modal%22%2C%22blocks%22%3A%5B%7B%22type%22%3A%22input%22%2C%22block_id%22%3A%22TitleBlock%22%2C%22label%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Title%22%2C%22emoji%22%3Atrue%7D%2C%22optional%22%3Afalse%2C%22element%22%3A%7B%22type%22%3A%22plain_text_input%22%2C%22action_id%22%3A%22title%22%2C%22placeholder%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22What+is+your+question%3F%22%2C%22emoji%22%3Atrue%7D%2C%22max_length%22%3A75%7D%7D%2C%7B%22type%22%3A%22input%22%2C%22block_id%22%3A%22ChannelBlock%22%2C%22label%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Channel%28s%29%22%2C%22emoji%22%3Atrue%7D%2C%22optional%22%3Afalse%2C%22element%22%3A%7B%22type%22%3A%22channels_select%22%2C%22action_id%22%3A%22channel%22%2C%22placeholder%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Where+should+the+poll+be+sent%3F%22%2C%22emoji%22%3Atrue%7D%7D%7D%2C%7B%22type%22%3A%22input%22%2C%22block_id%22%3A%22AnswerBlock1%22%2C%22label%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Option+1%22%2C%22emoji%22%3Atrue%7D%2C%22optional%22%3Afalse%2C%22element%22%3A%7B%22type%22%3A%22plain_text_input%22%2C%22action_id%22%3A%22option_1%22%2C%22placeholder%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Available+option%22%2C%22emoji%22%3Atrue%7D%2C%22max_length%22%3A75%7D%7D%2C%7B%22type%22%3A%22input%22%2C%22block_id%22%3A%22AnswerBlock2%22%2C%22label%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Option+2%22%2C%22emoji%22%3Atrue%7D%2C%22optional%22%3Afalse%2C%22element%22%3A%7B%22type%22%3A%22plain_text_input%22%2C%22action_id%22%3A%22option_2%22%2C%22placeholder%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Available+option%22%2C%22emoji%22%3Atrue%7D%2C%22max_length%22%3A75%7D%7D%2C%7B%22type%22%3A%22actions%22%2C%22block_id%22%3A%220KP%22%2C%22elements%22%3A%5B%7B%22type%22%3A%22button%22%2C%22action_id%22%3A%22add_option%22%2C%22text%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Add+another+option%22%2C%22emoji%22%3Atrue%7D%2C%22value%22%3A%223%22%7D%2C%7B%22type%22%3A%22button%22%2C%22action_id%22%3A%22delete_option%22%2C%22text%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Delete+option%22%2C%22emoji%22%3Atrue%7D%2C%22value%22%3A%222%22%7D%5D%7D%5D%2C%22private_metadata%22%3A%22%22%2C%22callback_id%22%3A%22create_questionnaire%22%2C%22state%22%3A%7B%22values%22%3A%7B%22TitleBlock%22%3A%7B%22title%22%3A%7B%22type%22%3A%22plain_text_input%22%2C%22value%22%3A%22Automatic+deployment+test+questionnaire%22%7D%7D%2C%22ChannelBlock%22%3A%7B%22channel%22%3A%7B%22type%22%3A%22channels_select%22%2C%22selected_channel%22%3A%22C015CJXTKL5%22%7D%7D%2C%22AnswerBlock1%22%3A%7B%22option_1%22%3A%7B%22type%22%3A%22plain_text_input%22%2C%22value%22%3A%22Kyll%5Cu00e4%22%7D%7D%2C%22AnswerBlock2%22%3A%7B%22option_2%22%3A%7B%22type%22%3A%22plain_text_input%22%2C%22value%22%3A%22Ei%22%7D%7D%7D%7D%2C%22hash%22%3A%221592219605.0f5e31c4%22%2C%22title%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Create+questionnaire%22%2C%22emoji%22%3Atrue%7D%2C%22clear_on_close%22%3Afalse%2C%22notify_on_close%22%3Afalse%2C%22close%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Cancel%22%2C%22emoji%22%3Atrue%7D%2C%22submit%22%3A%7B%22type%22%3A%22plain_text%22%2C%22text%22%3A%22Submit%22%2C%22emoji%22%3Atrue%7D%2C%22previous_view_id%22%3Anull%2C%22root_view_id%22%3A%22V015E53EWSW%22%2C%22app_id%22%3A%22A012L17EV33%22%2C%22external_id%22%3A%22%22%2C%22app_installed_team_id%22%3A%22T077KUF1P%22%2C%22bot_id%22%3A%22B011TFVLBC7%22%7D%2C%22response_urls%22%3A%5B%5D%7D";
            var content = new StringContent(payload);

            var result = await client.PostAsync(_url, content);
            result.EnsureSuccessStatusCode();
        }
    }
}