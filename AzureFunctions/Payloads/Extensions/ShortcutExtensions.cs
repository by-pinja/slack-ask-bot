using System.Collections.Generic;
using System.Linq;
using CloudLib.Models;

namespace AzureFunctions.Payloads
{
    public static class ShortcutExtensions
    {
        public static dynamic GetOpenListOfQuestionnairesPayload(this Shortcut shortcut, IEnumerable<QuestionnaireEntity> questionnaires, string callbackId)
        {
            return new
            {
                trigger_id = shortcut.TriggerId,
                view = new
                {
                    type = "modal",
                    callback_id = callbackId,
                    title = new
                    {
                        type = "plain_text",
                        text = callbackId == "get_answers" ? "Get answers" : "Delete a questionnaire",
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

        public static dynamic GetNoQuestionnairesAvailablePayload(this Shortcut shortcut)
        {
            return new
            {
                trigger_id = shortcut.TriggerId,
                view = new
                {
                    type = "modal",
                    callback_id = "no_available_questionnaires",
                    title = new
                    {
                        type = "plain_text",
                        text = $"Unavailable",
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
                                text = ":desert_island: There are no available questionnaires.",
                                emoji = true
                            }
                        }
                    }
                }
            };
        }

        public static dynamic GetConfirmDeleteAllPayload(this Shortcut shortcut)
        {
            return new
            {
                trigger_id = shortcut.TriggerId,
                view = new
                {
                    type = "modal",
                    callback_id = "delete_questionnaires",
                    title = new
                    {
                        type = "plain_text",
                        text = $"Delete all data",
                    },
                    submit = new
                    {
                        type = "plain_text",
                        text = "Yes",
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
                                text = ":warning: Are you sure you would like to delete all questionnaires and answers?",
                                emoji = true
                            }
                        }
                    }
                }
            };
        }

        public static dynamic GetOpenCreateQuestionnairesPayload(this Shortcut shortcut, dynamic mainViewPayload)
        {
            return new
            {
                trigger_id = shortcut.TriggerId,
                view = mainViewPayload
            };
        }
    }
}