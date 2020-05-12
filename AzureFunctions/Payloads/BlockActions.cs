using System;
using System.Linq;
using System.Threading.Tasks;
using AskBotCore;
using CloudLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlackLib;
using SlackLib.Messages;

namespace AzureFunctions.Payloads
{
    public class BlockActions : PayloadBase, IPayload
    {
        public BlockActions(SlackClient slackClient, AskBotControl control, IStorage storage, ILogger<PayloadBase> logger) : base(slackClient, control, storage, logger)
        {
        }

        [JsonProperty("trigger_id")]
        private string TriggerId { get; set; }

        private WithText Message { get; set; }

        private Channel Channel { get; set; }

        private User User { get; set; }

        private Action[] Actions { get; set; }

        private dynamic GetOpenQuestionnaireViewPayload(Questionnaire questionnaire)
        {
            return new
            {
                trigger_id = TriggerId,
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
        }

        public async Task<IActionResult> Handle()
        {
            _logger.LogInformation("Model open request received from {channel} by {answerer}", Channel.Name, User.Username);
            var dtoQuestionnaire = (await _storage.GetQuestionnaires(Actions[0].Value)).FirstOrDefault();

            if (dtoQuestionnaire is null)
            {
                _logger.LogCritical("Error retrieving the questionnaire for callback id: {callbackId}.", Actions[0].Value);
                throw new Exception("Can not retrieve the correct questionnaire.");
            }

            var questionnaire = new Questionnaire()
            {
                QuestionId = dtoQuestionnaire.QuestionnaireId,
                Question = dtoQuestionnaire.Question,
                AnswerOptions = dtoQuestionnaire.AnswerOptions.Split(";")
            };

            var viewPayload = GetOpenQuestionnaireViewPayload(questionnaire);

            _logger.LogInformation("Opening slack model to answer the questionnaire.");
            await _slackClient.OpenModelView(viewPayload);

            return new OkResult();
        }
    }

    public class Action
    {
        [JsonProperty("action_id")]
        public string ActionId { get; set; }

        [JsonProperty("block_id")]
        public string BlockId { get; set; }
        public string Value { get; set; }
    }
}