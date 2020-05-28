using System.Linq;
using SlackLib.Messages;

namespace AzureFunctions.Payloads
{
    public static class BlockActionExtensions
    {
        public static dynamic GetOpenQuestionnaireViewPayload(this BlockAction action, Questionnaire questionnaire)
        {
            return new
            {
                trigger_id = action.TriggerId,
                view = new
                {
                    type = "modal",
                    callback_id = "open_questionnaire",
                    private_metadata = questionnaire.QuestionId,
                    title = new
                    {
                        type = "plain_text",
                        text = "Submit answer",
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

        public static dynamic GetRemovedQuestionnaireViewPayload(this BlockAction action)
        {
            return new
            {
                trigger_id = action.TriggerId,
                view = new
                {
                    type = "modal",
                    callback_id = "questionnaire_not_found",
                    title = new
                    {
                        type = "plain_text",
                        text = "Error",
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
                                text = ":panic: The questionnaire you are attempting to answer could not be found.",
                                emoji = true
                            }
                        }
                    }
                }
            };
        }

        public static dynamic GetAddOptionToQuestionnairePayload(this BlockAction action, dynamic mainViewPayload)
        {
            return new
            {
                view_id = action.View.Id,
                hash = action.View.Hash,
                view = mainViewPayload
            };
        }
    }
}