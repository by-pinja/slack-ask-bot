using System.Linq;
using CloudLib.Models;
using SlackLib.Objects;
using SlackLib.Requests;

namespace AzureFunctions.Payloads
{
    public static class BlockActionExtensions
    {
        public static ViewsOpenRequest GetOpenQuestionnaireViewPayload(this BlockAction action, QuestionnaireEntity questionnaire, string? previousAnswer)
        {
            var previousAnswerExplanation = previousAnswer is null ? string.Empty : $" Previous answer was: {previousAnswer}";

            return new ViewsOpenRequest
            {
                TriggerId = action.TriggerId,
                View = new ViewObject
                {
                    Type = "modal",
                    CallbackId = "open_questionnaire",
                    PrivateMetadata = questionnaire.QuestionnaireId,
                    Title = new PlainTextObject
                    {
                        Text = "Submit answer"
                    },
                    Submit = new PlainTextObject
                    {
                        Text = "Submit"
                    },
                    Close = new PlainTextObject
                    {
                        Text = "Cancel"
                    },
                    Blocks = new dynamic[]
                    {
                        new
                        {
                            type = "section",
                            text = new {
                                type = "mrkdwn",
                                text = $"A new answer will replace your previous one.{previousAnswerExplanation}"
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

        public static ViewsOpenRequest GetRemovedQuestionnaireViewPayload(this BlockAction action)
        {
            return new ViewsOpenRequest
            {
                TriggerId = action.TriggerId,
                View = new ViewObject
                {
                    Type = "modal",
                    CallbackId = "questionnaire_not_found",
                    Title = new PlainTextObject
                    {
                        Text = "Unavailable"
                    },
                    Close = new PlainTextObject
                    {
                        Text = "Close"
                    },
                    Blocks = new[]
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

        public static ViewsUpdateRequest GetAddOptionToQuestionnairePayload(this BlockAction action, ViewObject mainViewPayload)
        {
            return new ViewsUpdateRequest
            {
                ViewId = action.View.Id,
                Hash = action.View.Hash,
                View = mainViewPayload
            };
        }
    }
}
