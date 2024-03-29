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
                Blocks = new BlockObject[]
                {
                    new SectionObject
                    {
                        BlockId = questionnaire.QuestionnaireId,
                        Text = new MarkdownTextObject
                        {
                            Text = questionnaire.Question
                        }
                    },
                    new ActionBlock
                    {
                        Elements = new BlockElement[]
                        {
                            new ButtonElement
                            {
                                ActionId = "open_questionnaire",
                                Value = questionnaire.QuestionnaireId,
                                Text = new PlainTextObject
                                {
                                    Text = "Answer"
                                }
                            },
                            new ButtonElement
                            {
                                ActionId = "get_answers",
                                Value = questionnaire.QuestionnaireId,
                                Text = new PlainTextObject
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
                Blocks = new[]
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
            var titleAndChannelBlocks = new object[]{ new InputObject
            {
                BlockId = "TitleBlock",
                Element = new PlainTextInputElement
                {
                    ActionId = "title",
                    Placeholder = new PlainTextObject
                    {
                        Text = "What is your question?"
                    },
                    MaxLength = 75
                },
                Label = new PlainTextObject
                {
                    Text = "Title"
                }
            },
            new InputObject
            {
                BlockId = "ChannelBlock",
                Element = new ConversationsListElement
                {
                    ActionId = "channel",
                    Placeholder = new PlainTextObject
                    {
                        Text = "Where should the poll be sent?"
                    }
                },
                Label = new PlainTextObject
                {
                    Text = "Channel(s). If channel is private, bot must be invited to the channel before creating the questionnaire."
                }
            }};

            var answerBlocks = new object[numberOfOptions];
            for (var i = 0; i < numberOfOptions; i++)
            {
                answerBlocks[i] = new InputObject
                {
                    BlockId = $"AnswerBlock{i + 1}",
                    Element = new PlainTextInputElement
                    {
                        ActionId = $"option_{i + 1}",
                        Placeholder = new PlainTextObject
                        {
                            Text = "Available option"
                        },
                        MaxLength = 75
                    },
                    Label = new PlainTextObject
                    {
                        Text = $"Option {i + 1}"
                    }
                };
            }

            var buttonBlocks = new ActionBlock[1] {
                new ActionBlock
                {
                    Elements = new BlockElement[]
                    {
                        new ButtonElement
                        {
                            ActionId = "add_option",
                            Text = new PlainTextObject
                            {
                                Text = "Add another option"
                            },
                            Value = $"{numberOfOptions + 1}"
                        },
                        new ButtonElement
                        {
                            ActionId = "delete_option",
                            Text = new PlainTextObject
                            {
                                Text = "Delete option"
                            },
                            Value = numberOfOptions <= 2 ? "2" : $"{numberOfOptions - 1}"
                        }
                    }
                }
            };

            var blocks = new BlockObject[titleAndChannelBlocks.Length + answerBlocks.Length + buttonBlocks.Length];
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
