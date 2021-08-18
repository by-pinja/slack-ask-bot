using System.Collections.Generic;
using System.Linq;
using CloudLib.Models;
using SlackLib.Interactions;
using SlackLib.Objects;
using SlackLib.Requests;

namespace AzureFunctions.Payloads
{
    public static class ShortcutExtensions
    {
        public static ViewsOpenRequest GetOpenListOfQuestionnairesPayload(this Shortcut shortcut, IEnumerable<QuestionnaireEntity> questionnaires, string callbackId)
        {
            return new ViewsOpenRequest
            {
                TriggerId = shortcut.TriggerId,
                View = new ViewObject
                {
                    Type = "modal",
                    CallbackId = callbackId,
                    Title = new PlainTextObject
                    {
                        Text = callbackId == "get_answers" ? "Get answers" : "Delete a questionnaire"
                    },
                    Submit = new PlainTextObject
                    {
                        Text = "Submit"
                    },
                    Close = new PlainTextObject
                    {
                        Text = "Cancel"
                    },
                    Blocks = new[]
                    {
                        new
                        {
                            type = "input",
                            block_id = "SelectBlock",
                            element = new
                            {
                                type = "static_select",
                                action_id = "questionnaires",
                                placeholder = new
                                {
                                    type = "plain_text",
                                    text = "Select a questionnaire"
                                },
                                options = questionnaires.Select(option =>
                                {
                                    return new
                                    {
                                        text = new
                                        {
                                            type = "plain_text",
                                            text = option.Question
                                        },
                                        value = option.QuestionnaireId
                                    };
                                })
                            },
                            label = new
                            {
                                type = "plain_text",
                                text = "Questionnaire"
                            }
                        }
                    }
                }
            };
        }

        public static ViewsOpenRequest GetNoQuestionnairesAvailablePayload(this Shortcut shortcut)
        {
            return new ViewsOpenRequest
            {
                TriggerId = shortcut.TriggerId,
                View = new ViewObject
                {
                    Type = "modal",
                    CallbackId = "no_available_questionnaires",
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
                        new SectionObject
                        {
                            Text = new PlainTextObject
                            {
                                Text = ":desert_island: There are no available questionnaires.",
                                Emoji = true
                            }
                        }
                    }
                }
            };
        }

        public static ViewsOpenRequest GetOpenCreateQuestionnairesPayload(this Shortcut shortcut, ViewObject mainViewPayload)
        {
            return new ViewsOpenRequest
            {
                TriggerId = shortcut.TriggerId,
                View = mainViewPayload
            };
        }
    }
}
