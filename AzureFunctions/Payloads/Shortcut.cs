using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskBotCore;
using CloudLib;
using CloudLib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlackLib;

namespace AzureFunctions.Payloads
{
    public class Shortcut : PayloadBase, IPayload
    {
        public Shortcut(SlackClient slackClient, AskBotControl control, IStorage storage, ILogger<PayloadBase> logger) : base(slackClient, control, storage, logger)
        {
        }

        [JsonProperty("callback_id")]
        private string CallbackId { get; set; }

        private User User { get; set; }

        [JsonProperty("trigger_id")]
        private string TriggerId { get; set; }

        private dynamic GetOpenListOfQuestionnairesPayload(IEnumerable<QuestionnaireEntity> questionnaires)
        {
            return new
            {
                trigger_id = TriggerId,
                view = new
                {
                    type = "modal",
                    callback_id = "get_answers",
                    title = new
                    {
                        type = "plain_text",
                        text = $"Select a questionnaire.",
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

        private dynamic GetOpenCreateQuestionnairesPayload()
        {
            return new
            {
                trigger_id = TriggerId,
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

        public async Task<IActionResult> Handle()
        {
            _logger.LogInformation("Shortcut request received from {user} with callback ID: {callback}", User.Username, CallbackId);

            dynamic payload;
            switch (CallbackId)
            {
                case "create_questionnaire":
                    payload = GetOpenCreateQuestionnairesPayload();

                    _logger.LogInformation("Opening slack model to create questionnaire.");
                    await _slackClient.OpenModelView(payload);
                    break;
                case "get_answers":
                    var questionnaires = await _control.GetQuestionnaires().ConfigureAwait(false);
                    payload = GetOpenListOfQuestionnairesPayload(questionnaires);

                    _logger.LogInformation("Opening slack model to list the questionnaires available.");
                    await _slackClient.OpenModelView(payload);
                    break;
                default:
                    throw new NotImplementedException($"Unknown shortcut callback id: {CallbackId}.");
            }

            return new OkResult();
        }
    }
}