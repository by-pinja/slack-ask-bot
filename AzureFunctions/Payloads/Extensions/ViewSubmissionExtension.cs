using System.Linq;
using SlackLib.Messages;

namespace AzureFunctions.Payloads
{
    public static class ViewSubmissionExtensions
    {
        public static dynamic GetUpdateModelWithAnswersPayload(this ViewSubmission viewSubmission, QuestionnaireResult questionnaireResult)
        {
            var blockSection = new object[] {
                        new
                        {
                            type = "section",
                            text = new
                            {
                                type = "plain_text",
                                text = $":wave: The votes are in. {questionnaireResult.Question}",
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
                        text = "Results",
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

        public static dynamic GetDeletedQuestionnairePayload(this ViewSubmission viewSubmission, string questionnaireTitle)
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

        public static dynamic GetDeletedQuestionnairesPayload(this ViewSubmission viewSubmission)
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

        public static dynamic GetConfirmAnsweredPayload(this ViewSubmission viewSubmission)
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
}