using System.Linq;
using CloudLib.Models;
using SlackLib.Messages;
using SlackLib.Objects;
using SlackLib.Requests;

namespace AskBotCore
{
    public static class PayloadUtility
    {

        public static ChatPostMessageRequest PlainMessagePayload(string channel, string message)
        {
            return new ChatPostMessageRequest
            {
                Channel = channel,
                Text = message
            };
        }

        /// <summary>
        /// Payload for message which updates chat message to host questionnaire.
        /// This is done after posting to assure that questionnaire can be posted before
        /// actually creating the questionniare.
        /// 
        /// Questionnaire posting can fail if bot doesnt have correct permission or
        /// is not in correct (private) channel.
        /// </summary>
        /// <param name="questionnaire"></param>
        /// <returns></returns>
        public static ChatUpdateRequest GetQuestionnaireUpdatePostPayload(string channel, string timestamp, QuestionnaireEntity questionnaire)
        {
            return new ChatUpdateRequest
            {
                Channel = channel,
                Timestamp = timestamp,
                Blocks = new object[]
                {
                    new
                    {
                        type = "section",
                        block_id = questionnaire.QuestionnaireId,
                        text = new
                        {
                            type = "mrkdwn",
                            text = questionnaire.Question
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
                                action_id = "open_questionnaire",
                                value = questionnaire.QuestionnaireId,
                                text = new
                                {
                                    type = "plain_text",
                                    text = "Answer"
                                }
                            }
                        }
                    }
                }
            };
        }

        public static dynamic GetUpdateModelWithAnswersPayload(QuestionnaireResult questionnaireResult)
        {
            var blockSection = new object[] {
                new
                {
                    type = "section",
                    text = new
                    {
                        type = "plain_text",
                        text = $":wave: The votes are in. {questionnaireResult.Question}",
                        emoji = true
                    }
                }
            };

            var answers = questionnaireResult.Answers.Select(kvp =>
                {
                    return (object)new
                    {
                        type = "section",
                        text = new
                        {
                            type = "plain_text",
                            text = $"\"{kvp.Key}\": {kvp.Value} votes.",
                        }
                    };
                });

            return new
            {
                response_action = "update",
                view = new
                {
                    type = "modal",
                    callback_id = "display_answers",
                    title = new
                    {
                        type = "plain_text",
                        text = "Results",
                    },
                    close = new
                    {
                        type = "plain_text",
                        text = "Close",
                    },
                    blocks = blockSection.Concat(answers)
                }
            };
        }

        public static dynamic GetDeletedQuestionnairePayload(string questionnaireTitle)
        {
            return new
            {
                response_action = "update",
                view = new
                {
                    type = "modal",
                    callback_id = "deleted_questionnaire",
                    title = new
                    {
                        type = "plain_text",
                        text = "Questionnaire deleted",
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
                                text = $":+1: The questionnaire \"{questionnaireTitle}\" and the answers have been deleted.",
                                emoji = true
                            }
                        }
                    }
                }
            };
        }

        public static dynamic GetDeletedQuestionnairesPayload()
        {
            return new
            {
                response_action = "update",
                view = new
                {
                    type = "modal",
                    callback_id = "delete_questionnaires",
                    title = new
                    {
                        type = "plain_text",
                        text = $"Deleted all data",
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
                                text = ":+1: All questionnaires and answers have been deleted.",
                                emoji = true
                            }
                        }
                    }
                }
            };
        }

        public static dynamic GetConfirmAnsweredPayload(string answer)
        {
            return new
            {
                response_action = "update",
                view = new
                {
                    type = "modal",
                    callback_id = "confirm_answered",
                    title = new
                    {
                        type = "plain_text",
                        text = "Answer Submitted",
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
                                text = $":partyparrot: Your answer '{answer}' has been successfully submitted.",
                                emoji = true
                            }
                        }
                    }
                }
            };
        }

        public static ViewObject GetCreateQuestionnaireMainPayload(int numberOfOptions = 2)
        {
            var titleAndChannelBlocks = new object[]{ new
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
                        text = "What is your question?"
                    },
                    max_length = 75
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
                    type = "conversations_select",
                    action_id = "channel",
                    placeholder = new
                    {
                        type = "plain_text",
                        text = "Where should the poll be sent?"
                    },
                },
                label = new
                {
                    type = "plain_text",
                    text = "Channel(s). If channel is private, bot must be invited to the channel before creating the questionnaire."
                }
            }};

            var answerBlocks = new object[numberOfOptions];
            for (var i = 0; i < numberOfOptions; i++)
            {
                answerBlocks[i] = new
                {
                    type = "input",
                    block_id = $"AnswerBlock{i + 1}",
                    element = new
                    {
                        type = "plain_text_input",
                        action_id = $"option_{i + 1}",
                        placeholder = new
                        {
                            type = "plain_text",
                            text = "Available option"
                        },
                        max_length = 75
                    },
                    label = new
                    {
                        type = "plain_text",
                        text = $"Option {i + 1}"
                    }
                };
            }

            var buttonBlocks = new object[1] {
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
                                text = "Add another option"
                            },
                            value = $"{numberOfOptions + 1}"
                        },
                        new
                        {
                            type = "button",
                            action_id = "delete_option",
                            text = new
                            {
                                type = "plain_text",
                                text = "Delete option"
                            },
                            value = numberOfOptions <= 2 ? "2" : $"{numberOfOptions - 1}"
                        }
                    }
                }
            };

            var blocks = new object[titleAndChannelBlocks.Length + answerBlocks.Length + buttonBlocks.Length];
            titleAndChannelBlocks.CopyTo(blocks, 0);
            answerBlocks.CopyTo(blocks, titleAndChannelBlocks.Length);
            buttonBlocks.CopyTo(blocks, titleAndChannelBlocks.Length + answerBlocks.Length);

            return new ViewObject
            {
                Type = "modal",
                CallbackId = "create_questionnaire",
                Title = new
                {
                    type = "plain_text",
                    text = "Create questionnaire",
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
                Blocks = blocks
            };
        }
    }
}
