using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackLib.Messages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using CloudLib.Models;

namespace SlackLib
{
    /// <summary>
    /// Wrapper for Slack API
    /// </summary>
    public class SlackClient
    {
        private readonly ILogger<SlackClient> _logger;
        private readonly SlackResponseParser _slackResponseParser;
        private readonly HttpClient _client;

        public SlackClient(ILogger<SlackClient> logger, SlackClientSettings slackClientSettings, SlackResponseParser slackResponseParser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _slackResponseParser = slackResponseParser ?? throw new ArgumentNullException(nameof(slackResponseParser));
            var client = new HttpClient();
            //if (_client is null) throw new NullReferenceException("Http client uninitialized in static constructor.");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {slackClientSettings?.BearerToken ?? throw new ArgumentNullException(nameof(slackClientSettings))}");
            _client = client;
        }

        ~SlackClient()
        {
            _client.Dispose();
        }

        public async Task PostQuestionnaire(Questionnaire questionnaire, string channel)
        {
            _logger.LogInformation("Posting questionnaire message");

            var messagePayload = new
            {
                channel,
                text = "PostQuestionnaire",
                blocks = new[]
                {
                    Section(questionnaire),
                    new
                    {
                        type = "actions",
                        elements = new[]
                        {
                            new
                            {
                                type = "button",
                                value = questionnaire.QuestionId,
                                text = new
                                {
                                    type = "plain_text",
                                    text = "Vastaa"
                                }
                            }
                        }
                    }
                }
            };

            await ExecuteSlackCall(messagePayload, "https://slack.com/api/chat.postMessage", "posting questionnaire message.");
        }

        private dynamic Section(Questionnaire questionnaire)
        {
            return new
            {
                type = "section",
                block_id = questionnaire.QuestionId,
                text = new
                {
                    type = "mrkdwn",
                    text = questionnaire.Question
                }
            };
        }

        // private dynamic AnswerOptions(string[] options)
        // {
        //     return new
        //     {
        //         type = "actions",
        //         elements = options.Select(option => new
        //         {
        //             type = "button",
        //             action_id = "Open questionnaire",
        //             text = new
        //             {
        //                 type = "plain_text",
        //                 text = option,
        //                 emoji = true
        //             }
        //         }).ToArray()
        //     };
        // }

        public async Task OpenAnswerView(string triggerId, Questionnaire questionnaire)
        {
            _logger.LogInformation("Opening questionnaire to answer");

            var viewPayload = new
            {
                trigger_id = triggerId,
                view = new
                {
                    type = "modal",
                    callback_id = "open_questionnaire",
                    private_metadata = questionnaire.QuestionId,
                    title = new
                    {
                        type = "plain_text",
                        text = questionnaire.Question,
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

            await ExecuteSlackCall(viewPayload, "https://slack.com/api/views.open", "opening answer view");
        }

        public async Task OpenCreateQuestionnaireView(string triggerId)
        {
            _logger.LogInformation("Opening create questionnaire model.");

            var viewPayload = new
            {
                trigger_id = triggerId,
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

            await ExecuteSlackCall(viewPayload, "https://slack.com/api/views.open", "creating questionnaire").ConfigureAwait(false);
        }

        public async Task OpenGetAnswersView(IEnumerable<QuestionnaireEntity> questionnaires, string triggerId)
        {
            _logger.LogInformation("Opening get answers view.");

            var viewPayload = new
            {
                trigger_id = triggerId,
                view = new
                {
                    type = "modal",
                    callback_id = "get_answers",
                    title = new
                    {
                        type = "plain_text",
                        text = $"Which questionnaire would you like to get the answers for?",
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

            await ExecuteSlackCall(viewPayload, "https://slack.com/api/views.open", "opening get answers view").ConfigureAwait(false);
        }

        public async Task UpdateViewWithAnswers(QuestionnaireResult questionnaireResult, string viewId, string hash)
        {
            _logger.LogInformation("Updating the get answers view.");

            var viewPayload = new
            {
                view_id = viewId,
                hash,
                view = new
                {
                    type = "modal",
                    callback_id = "display_answers",
                    title = new
                    {
                        type = "plain_text",
                        text = questionnaireResult.Question,
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
                            type = "section",
                            text = new
                            {
                                type = "plain_text",
                                text = ":wave: The votes are in.",
                                emoji = true
                            }
                        },
                        questionnaireResult.Answers.Select(kvp =>
                        {
                            return new
                            {
                                type = "section",
                                text = new
                                {
                                    type = "plain_text",
                                    text = $"\"{kvp.Key}\": {kvp.Value} votes.",
                                }
                            };
                        })
                    }
                }
            };

            await ExecuteSlackCall(viewPayload, "https://slack.com/api/views.update", "opening answers for questionnaire view").ConfigureAwait(false);
        }

        private async Task ExecuteSlackCall(dynamic payload, string address, string action)
        {
            _logger.LogDebug("Executing slack request: {action}. Address: {address}", action, address);

            try
            {
                string serializedPayload = JsonConvert.SerializeObject(payload);
                _logger.LogDebug("Serialised: {payload}.", serializedPayload);
                using (var requestContent = new StringContent(serializedPayload, Encoding.UTF8, "application/json"))
                {
                    var response = await _client.PostAsync(address, requestContent).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    _logger.LogTrace("Request successful, checking content. Content: {0}", content);
                    var parsed = _slackResponseParser.Parse(content);
                    if (parsed == null || !parsed.Ok)
                    {
                        _logger.LogTrace("Request successful but SlackAPI error. Error message: {0}", parsed?.Error ?? "Could not parse slack response.");
                        throw new SlackLibException($"Something went wrong while {action}.");
                    }
                }
            }
            catch (HttpRequestException)
            {
                _logger.LogTrace("Failed Http request to Slack API.");
                throw;
            }
            catch (JsonSerializationException)
            {
                _logger.LogTrace("Failed to serialise the payload into json.");
                throw;
            }
        }
    }
}
