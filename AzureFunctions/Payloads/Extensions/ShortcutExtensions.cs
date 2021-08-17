using System.Collections.Generic;
using System.Linq;
using CloudLib.Models;
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
                    Title = new
                    {
                        type = "plain_text",
                        text = callbackId == "get_answers" ? "Get answers" : "Delete a questionnaire",
                    },
                    Submit = new
                    {
                        type = "plain_text",
                        text = "Submit",
                    },
                    Close = new
                    {
                        type = "plain_text",
                        text = "Cancel",
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
                    Title = new
                    {
                        type = "plain_text",
                        text = $"Unavailable",
                    },
                    Close = new
                    {
                        type = "plain_text",
                        text = "Close",
                    },
                    Blocks = new[]
                    {
                        new
                        {
                            type = "section",
                            text = new
                            {
                                type = "plain_text",
                                text = ":desert_island: There are no available questionnaires.",
                                emoji = true
                            }
                        }
                    }
                }
            };
        }

        public static ViewsOpenRequest GetConfirmDeleteAllPayload(this Shortcut shortcut)
        {
            return new ViewsOpenRequest
            {
                TriggerId = shortcut.TriggerId,
                View = new ViewObject
                {
                    Type = "modal",
                    CallbackId = "delete_questionnaires",
                    Title = new
                    {
                        type = "plain_text",
                        text = "Delete all data",
                    },
                    Submit = new
                    {
                        type = "plain_text",
                        text = "Yes",
                    },
                    Close = new
                    {
                        type = "plain_text",
                        text = "Close",
                    },
                    Blocks = new[]
                    {
                        new
                        {
                            type = "section",
                            text = new
                            {
                                type = "plain_text",
                                text = ":warning: Are you sure you would like to delete all questionnaires and answers?",
                                emoji = true
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
