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
                        text = $"Error",
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
                                text = ":panic: There are no available questionnaires.",
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

        public static dynamic GetOpenCreateQuestionnairesPayload(this Shortcut shortcut)
        {
            return new
            {
                trigger_id = shortcut.TriggerId,
                view = new
                {
                    type = "modal",
                    callback_id = "create_questionnaire",
                    title = new
                    {
                        type = "plain_text",
                        text = "Create questionnaire",
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
                    blocks = new object[]
                    {
                        new
                        {
                            type = "input",
                            block_id = "TitleBlock",
                            element = new
                            {
                                type = "plain_text_input",
                                action_id = "title",
                                placeholder = new
                                {
                                    type = "plain_text",
                                    text = "What do you want to ask of the world?"
                                }
                            },
                            label = new
                            {
                                type = "plain_text",
                                text = "Title"
                            }
                        },
                        new
                        {
                            type = "input",
                            block_id = "ChannelBlock",
                            element = new
                            {
                                type = "channels_select",
                                action_id = "channels",
                                placeholder = new
                                {
                                    type = "plain_text",
                                    text = "Where should the poll be sent?"
                                }
                            },
                            label = new
                            {
                                type = "plain_text",
                                text = "Channel(s)"
                            }
                        },
                        new
                        {
                            type = "input",
                            block_id = "Answer1Block",
                            element = new
                            {
                                type = "plain_text_input",
                                action_id = "option_1",
                                placeholder = new
                                {
                                    type = "plain_text",
                                    text = "First option"
                                }
                            },
                            label = new
                            {
                                type = "plain_text",
                                text = "Option 1"
                            }
                        },
                        new
                        {
                            type = "input",
                            block_id = "Answer2Block",
                            element = new
                            {
                                type = "plain_text_input",
                                action_id = "option_2",
                                placeholder = new
                                {
                                    type = "plain_text",
                                    text = "How many options do they need, really?"
                                }
                            },
                            label = new
                            {
                                type = "plain_text",
                                text = "Option 2"
                            }
                        },
                        new
                        {
                            type = "actions",
                            elements = new[]
                            {
                                new
                                {
                                    type = "button",
                                    action_id = "add_option",
                                    text = new
                                    {
                                        type = "plain_text",
                                        text = "Add another option  "
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}