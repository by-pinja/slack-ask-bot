using System.Linq;
using CloudLib.Models;
using SlackLib.Interactions;
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
                    Blocks = new BlockObject[]
                    {
                        new SectionObject
                        {
                            Text = new MarkdownTextObject
                            {
                                Text = $"A new answer will replace your previous one.{previousAnswerExplanation}"
                            }
                        },
                        new InputObject
                        {
                            BlockId = "AnswerBlock",
                            Element = new StaticSelectElement
                            {
                                ActionId = "title",
                                Placeholder = new PlainTextObject
                                {
                                    Text = "Select an option"
                                },
                                Options = questionnaire.AnswerOptions.Select(option =>
                                {
                                    return new OptionObject
                                    {
                                        Text = new PlainTextObject
                                        {
                                            Text = option
                                        },
                                        Value = option
                                    };
                                }).ToArray()
                            },
                            Label = new PlainTextObject
                            {
                                Text = questionnaire.Question
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
                    Blocks = new BlockObject[]
                    {
                        new SectionObject
                        {
                            Text = new PlainTextObject
                            {
                                Text = ":disappointed: The questionnaire you are attempting to answer has closed.",
                                Emoji = true
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
