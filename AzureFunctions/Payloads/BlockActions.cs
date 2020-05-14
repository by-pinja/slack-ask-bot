using System.Linq;
using Newtonsoft.Json;
using SlackLib.Messages;

namespace AzureFunctions.Payloads
{
    public class BlockActions
    {
        [JsonProperty("trigger_id")]
        public string TriggerId { get; set; }

        public WithText Message { get; set; }

        public Channel Channel { get; set; }

        public User User { get; set; }

        public Action[] Actions { get; set; }

        public dynamic GetOpenQuestionnaireViewPayload(Questionnaire questionnaire)
        {
            return new
            {
                trigger_id = TriggerId,
                view = new
                {
                    type = "modal",
                    callback_id = "open_questionnaire",
                    private_metadata = questionnaire.QuestionId,
                    title = new
                    {
                        type = "plain_text",
                        text = questionnaire.Question,
                    },
                    submit = new
                    {
                        type = "plain_text",
                        text = "Submit",
                    },
                    close = new
                    {
                        type = "plain_text",
                        text = "Cancel",
                    },
                    blocks = new[]
                    {
                        new
                        {
                            type = "input",
                            block_id = "AnswerBlock",
                            element = new
                            {
                                type = "static_select",
                                action_id = "title",
                                placeholder = new
                                {
                                    type = "plain_text",
                                    text = "Select an option"
                                },
                                options = questionnaire.AnswerOptions.Select(option =>
                                {
                                    return new
                                    {
                                        text = new
                                        {
                                            type = "plain_text",
                                            text = option
                                        },
                                        value = option
                                    };
                                })
                            },
                            label = new
                            {
                                type = "plain_text",
                                text = questionnaire.Question
                            }
                        }
                    }
                }
            };
        }
    }

    public class Action
    {
        [JsonProperty("action_id")]
        public string ActionId { get; set; }

        [JsonProperty("block_id")]
        public string BlockId { get; set; }
        public string Value { get; set; }
    }
}