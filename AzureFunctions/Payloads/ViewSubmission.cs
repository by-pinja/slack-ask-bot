using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SlackLib.Messages;

namespace AzureFunctions.Payloads
{
    public class ViewSubmission
    {
        public View View { get; set; }
        public User User { get; set; }

        public dynamic GetUpdateModelWithAnswersPayload(QuestionnaireResult questionnaireResult)
        {
            var blockSection = new object[] {
                        new
                        {
                            type = "section",
                            text = new
                            {
                                type = "plain_text",
                                text = ":wave: The votes are in.",
                                emoji = true
                            }
                        }
            };

            var answers = questionnaireResult.Answers.Select(kvp =>
                        {
                            return (object)new
                            {
                                type = "section",
                                text = new
                                {
                                    type = "plain_text",
                                    text = $"\"{kvp.Key}\": {kvp.Value} votes.",
                                }
                            };
                        });

            return new
            {
                response_action = "update",
                view = new
                {
                    type = "modal",
                    callback_id = "display_answers",
                    title = new
                    {
                        type = "plain_text",
                        text = questionnaireResult.Question,
                    },
                    close = new
                    {
                        type = "plain_text",
                        text = "Close",
                    },
                    blocks = blockSection.Concat(answers)
                }
            };
        }

        public dynamic GetDeletedQuestionnairePayload(string questionnaireTitle)
        {
            return new
            {
                response_action = "update",
                view = new
                {
                    type = "modal",
                    callback_id = "deleted_questionnaire",
                    title = new
                    {
                        type = "plain_text",
                        text = "Questionnaire deleted",
                    },
                    close = new
                    {
                        type = "plain_text",
                        text = "Close",
                    },
                    blocks = new[]
                    {
                        new
                        {
                            type = "section",
                            text = new
                            {
                                type = "plain_text",
                                text = $":+1: The questionnaire \"{questionnaireTitle}\" and the answers have been deleted.",
                                emoji = true
                            }
                        }
                    }
                }
            };
        }

        public dynamic GetDeletedQuestionnairesPayload()
        {
            return new
            {
                response_action = "update",
                view = new
                {
                    type = "modal",
                    callback_id = "delete_questionnaires",
                    title = new
                    {
                        type = "plain_text",
                        text = $"Deleted all data",
                    },
                    close = new
                    {
                        type = "plain_text",
                        text = "Close",
                    },
                    blocks = new[]
                    {
                        new
                        {
                            type = "section",
                            text = new
                            {
                                type = "plain_text",
                                text = ":+1: All questionnaires and answers have been deleted.",
                                emoji = true
                            }
                        }
                    }
                }
            };
        }

        public dynamic GetConfirmAnsweredPayload()
        {
            return new
            {
                response_action = "update",
                view = new
                {
                    type = "modal",
                    callback_id = "confirm_answered",
                    title = new
                    {
                        type = "plain_text",
                        text = "Answer Submitted",
                    },
                    close = new
                    {
                        type = "plain_text",
                        text = "Close",
                    },
                    blocks = new[]
                    {
                        new
                        {
                            type = "section",
                            text = new
                            {
                                type = "plain_text",
                                text = ":partyparrot: Your answer has been successfully submitted.",
                                emoji = true
                            }
                        }
                    }
                }
            };
        }
    }

    public class View
    {
        public State State { get; set; }
        public string Id { get; set; }
        public string Hash { get; set; }

        public WithText Title { get; set; }

        [JsonProperty("callback_id")]
        public string CallbackId { get; set; }

        [JsonProperty("private_metadata")]
        public string PrivateMetadata { get; set; }
    }

    public class State
    {
        public Dictionary<string, Dictionary<string, Data>> values { get; set; }
    }

    public class Data
    {
        public string Value { get; set; }

        [JsonProperty("selected_option")]
        public SelectedOption SelectedOption { get; set; }

        [JsonProperty("selected_channel")]
        private string SelectedChannel { set { this.Value = value; } }
    }

    public class SelectedOption
    {
        public string Value { get; set; }
    }
}