using System.Linq;
using SlackLib.Messages;

namespace AzureFunctions.Payloads
{
    public static class BlockActionExtensions
    {
        public static dynamic GetOpenQuestionnaireViewPayload(this BlockAction action, Questionnaire questionnaire)
        {
            return new
            {
                trigger_id = action.TriggerId,
                view = new
                {
                    type = "modal",
                    callback_id = "open_questionnaire",
                    private_metadata = questionnaire.QuestionId,
                    title = new
                    {
                        type = "plain_text",
                        text = "Submit answer",
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
        }

        public static dynamic GetRemovedQuestionnaireViewPayload(this BlockAction action)
        {
            return new
            {
                trigger_id = action.TriggerId,
                view = new
                {
                    type = "modal",
                    callback_id = "questionnaire_not_found",
                    title = new
                    {
                        type = "plain_text",
                        text = "Error",
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
                                text = ":panic: The questionnaire you are attempting to answer could not be found.",
                                emoji = true
                            }
                        }
                    }
                }
            };
        }

        public static dynamic GetAddOptionToQuestionnairePayload(this BlockAction action, dynamic mainViewPayload)
        {
            // var answerOptionDictionaries = view.State.values.Where(d => d.Key.Contains("Answer")).Select(kvp => kvp.Value);
            // var currentAnswerOptions = answerOptionDictionaries.Select(d => d.First().Value.Value).ToArray();

            // var titleAndChannelBlocks = new object[]{ new
            // {
            //     type = "input",
            //     block_id = "TitleBlock",
            //     element = new
            //     {
            //         type = "plain_text_input",
            //         action_id = "title",
            //         placeholder = new
            //         {
            //             type = "plain_text",
            //             text = "What do you want to ask of the world?"
            //         },
            //         initial_value = view.State.values["TitleBlock"]["title"]?.Value ?? ""
            //     },
            //     label = new
            //     {
            //         type = "plain_text",
            //         text = "Title"
            //     }
            // },
            // new
            // {
            //     type = "input",
            //     block_id = "ChannelBlock",
            //     element = new
            //     {
            //         type = "channels_select",
            //         action_id = "channel",
            //         placeholder = new
            //         {
            //             type = "plain_text",
            //             text = "Where should the poll be sent?"
            //         },
            //         initial_option = view.State.values["ChannelBlock"]["channel"].Value ?? ""
            //     },
            //     label = new
            //     {
            //         type = "plain_text",
            //         text = "Channel(s)"
            //     }
            // }};

            // var numberOfAnswersNew = (int.Parse(view.PrivateMetadata) + 1);

            // var answerBlocks = new object[numberOfAnswersNew];
            // for (var i = 0; i < currentAnswerOptions.Length; i++)
            // {
            //     answerBlocks[i] = new
            //     {
            //         type = "input",
            //         block_id = $"AnswerBlock{i + 1}",
            //         element = new
            //         {
            //             type = "plain_text_input",
            //             action_id = $"option_{i + 1}",
            //             placeholder = new
            //             {
            //                 type = "plain_text",
            //                 text = "Available option"
            //             },
            //             initial_value = currentAnswerOptions[i]
            //         },
            //         label = new
            //         {
            //             type = "plain_text",
            //             text = $"Option {i + 1}"
            //         }
            //     };
            // }

            // for (var i = currentAnswerOptions.Length; i < numberOfAnswersNew; i++)
            // {
            //     answerBlocks[i] = new
            //     {
            //         type = "input",
            //         block_id = $"AnswerBlock{i + 1}",
            //         element = new
            //         {
            //             type = "plain_text_input",
            //             action_id = $"option_{i + 1}",
            //             placeholder = new
            //             {
            //                 type = "plain_text",
            //                 text = "Available option"
            //             },
            //         },
            //         label = new
            //         {
            //             type = "plain_text",
            //             text = $"Option {i + 1}"
            //         }
            //     };
            // }

            // var blocks = new object[answerBlocks.Length + 3];
            // titleAndChannelBlocks.CopyTo(blocks, 0);
            // answerBlocks.CopyTo(blocks, 2);
            // blocks[blocks.Length - 1] = new
            // {
            //     type = "actions",
            //     elements = new[]
            //     {
            //         new
            //         {
            //             type = "button",
            //             action_id = "add_option",
            //             text = new
            //             {
            //                 type = "plain_text",
            //                 text = "Add another option"
            //             }
            //         }
            //     }
            // };

            return new
            {
                view_id = action.View.Id,
                hash = action.View.Hash,
                view = mainViewPayload
            };
        }
    }
}