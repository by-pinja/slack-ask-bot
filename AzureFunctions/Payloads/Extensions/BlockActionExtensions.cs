using System.Linq;
using CloudLib.Models;

namespace AzureFunctions.Payloads
{
    public static class BlockActionExtensions
    {
        public static dynamic GetOpenQuestionnaireViewPayload(this BlockAction action, QuestionnaireEntity questionnaire, string? previousAnswer)
        {
            var topicText = previousAnswer is null ? string.Empty : $" Previous answer was: {previousAnswer}";

            return new
            {
                trigger_id = action.TriggerId,
                view = new
                {
                    type = "modal",
                    callback_id = "open_questionnaire",
                    private_metadata = questionnaire.QuestionnaireId,
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
                    blocks = new dynamic[]
                    {
                        new
                        {
                            type = "section",
                            text = new {
                                type = "mrkdwn",
                                text = $"A new answer will replace your previous one.{topicText}"
                            }
                        },
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
                        text = "Unavailable",
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
                                text = ":disappointed: The questionnaire you are attempting to answer has closed.",
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
