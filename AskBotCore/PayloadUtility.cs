using System.Collections.Generic;
using System.Text;
using CloudLib.Models;
using SlackLib.Interactions;
using SlackLib.Objects;
using SlackLib.Requests;

namespace AskBotCore
{
    public static class PayloadUtility
    {
        /// <summary>
        /// Payload for message which updates chat message to host questionnaire.
        /// This is done after posting to assure that questionnaire can be posted before
        /// actually creating the questionniare.
        /// 
        /// Questionnaire posting can fail if bot doesnt have correct permission or
        /// is not in correct (private) channel.
        /// </summary>
        public static ChatUpdateRequest GetQuestionnaireUpdatePostPayload(string channel, string timestamp, QuestionnaireEntity questionnaire)
        {
            return new ChatUpdateRequest
            {
                Channel = channel,
                Timestamp = timestamp,
                Blocks = new object[]
                {
                    new SectionObject
                    {
                        BlockId = questionnaire.QuestionnaireId,
                        Text = new MarkdownTextObject
                        {
                            Text = questionnaire.Question
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
                                text = new PlainTextObject
                                {
                                    Text = "Answer"
                                }
                            },
                            new
                            {
                                type = "button",
                                action_id = "get_answers",
                                value = questionnaire.QuestionnaireId,
                                text = new PlainTextObject
                                {
                                    Text = "Results"
                                }
                            }
                        }
                    }
                }
            };
        }

        public static ChatUpdateRequest GetQuestionnaireClosedPostUpdatePayload(string channel, string timestamp, QuestionnaireEntity questionnaire)
        {
            return new ChatUpdateRequest
            {
                Channel = channel,
                Timestamp = timestamp,
                Blocks = new object[]
                {
                    new SectionObject
                    {
                        BlockId = questionnaire.QuestionnaireId,
                        Text = new MarkdownTextObject
                        {
                            Text = $"{questionnaire.Question}\r\n*Questionnaire is now closed.*"
                        }
                    }
                 }
            };
        }

        public static string AnswersPostText(Dictionary<string, int> result)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Current answers:");
            foreach (var answer in result)
            {
                builder.AppendLine($"{answer.Key}: {answer.Value}");
            }
            return builder.ToString();
        }

        public static ViewSubmissionResponse GetDeletedQuestionnairePayload(string questionnaireTitle)
        {
            return new ViewSubmissionResponse
            {
                ResponseAction = "update",
                View = new ViewObject
                {
                    Type = "modal",
                    CallbackId = "deleted_questionnaire",
                    Title = new PlainTextObject
                    {
                        Text = "Questionnaire deleted"
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
                                Text = $":+1: The questionnaire \"{questionnaireTitle}\" and the answers have been deleted.",
                                Emoji = true
                            }
                        }
                    }
                }
            };
        }

        public static ViewSubmissionResponse GetConfirmAnsweredPayload(string answer)
        {
            return new ViewSubmissionResponse
            {
                ResponseAction = "update",
                View = new ViewObject
                {
                    Type = "modal",
                    CallbackId = "confirm_answered",
                    Title = new PlainTextObject
                    {
                        Text = "Answer Submitted"
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
                                Text = $":partyparrot: Your answer '{answer}' has been successfully submitted.",
                                Emoji = true
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
                Title = new PlainTextObject
                {
                    Text = "Create questionnaire"
                },
                Submit = new PlainTextObject
                {
                    Text = "Submit"
                },
                Close = new PlainTextObject
                {
                    Text = "Cancel"
                },
                Blocks = blocks
            };
        }
    }
}
