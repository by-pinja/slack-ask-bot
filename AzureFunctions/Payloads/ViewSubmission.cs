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
using SlackLib.Messages;

namespace AzureFunctions.Payloads
{
    public class ViewSubmission : PayloadBase, IPayload
    {
        public ViewSubmission(SlackClient slackClient, AskBotControl control, IStorage storage, ILogger<PayloadBase> logger) : base(slackClient, control, storage, logger)
        {
        }

        private View View { get; set; }
        private User User { get; set; }

        private dynamic GetUpdateModelWithAnswersPayload(QuestionnaireResult questionnaireResult)
        {
            var blockSection = new object[] {
                        new
                        {
                            type = "section",
                            text = new
                            {
                                type = "plain_text",
                                text = ":wave: The votes are in.",
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
                view_id = View.Id,
                View.Hash,
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
                    blocks = blockSection.Concat(answers)
                }
            };
        }

        public async Task<IActionResult> Handle()
        {
            _logger.LogInformation("View submission received.");

            switch (View.CallbackId)
            {
                case "create_questionnaire":
                    _logger.LogInformation("Creating and posting questionnaire received from {user}.", User.Username);

                    var channel = View.State.values["ChannelBlock"].First().Value.Value;
                    if (string.IsNullOrWhiteSpace(channel))
                    {
                        throw new Exception("channel is null");
                    }
                    var question = View.State.values["TitleBlock"]["title"].Value;
                    if (string.IsNullOrWhiteSpace(question))
                    {
                        throw new Exception("question is null");
                    }

                    var answerOptionDictionaries = View.State.values.Where(d => d.Key.Contains("Answer")).Select(kvp => kvp.Value);
                    var answerOptions = answerOptionDictionaries.Select(d => d.First().Value.Value).ToArray();

                    if (answerOptions.Count() == 0)
                    {
                        throw new Exception("answer option is empty.");
                    }

                    var questionnaire = new Questionnaire
                    {
                        QuestionId = Guid.NewGuid().ToString(),
                        Question = question,
                        AnswerOptions = answerOptions
                    };

                    await _control.CreateQuestionnaire(questionnaire, channel, DateTime.UtcNow).ConfigureAwait(false);
                    break;
                case "open_questionnaire":
                    _logger.LogInformation("Answer received from {answerer}.", User.Username);
                    var answer = View.State.values.First().Value.First().Value.SelectedOption.Value;
                    _logger.LogDebug("Answer: {answer}", answer);

                    var answerEntity = new AnswerEntity(View.PrivateMetadata, User.Username)
                    {
                        Answer = answer,
                        Answerer = User.Username,
                        //Channel = submission.Channel.Name,
                        Question = View.Title.Text,
                        QuestionnaireId = View.PrivateMetadata
                    };
                    await _storage.InsertOrMerge(answerEntity);
                    break;
                case "get_answers":
                    var selectedQuestionnaireId = View.State.values.First().Value.First().Value.SelectedOption.Value;
                    _logger.LogInformation("Get answers for questionnaire with ID: {questionnaire}.", selectedQuestionnaireId);
                    var questionnaireResult = await _control.GetAnswers(selectedQuestionnaireId).ConfigureAwait(false);
                    var payload = GetUpdateModelWithAnswersPayload(questionnaireResult);
                    return new JsonResult(payload);
                    //await _slackClient.UpdateViewWithAnswers(questionnaireResult, viewSubmission.View.Id, viewSubmission.View.Hash);
                    break;
                default:
                    throw new NotImplementedException($"Unknown view callback id: {View.CallbackId}.");
            }

            return new OkResult();
        }
    }

    public class View
    {
        public State State { get; set; }
        public string Id { get; set; }
        public string Hash { get; set; }

        public WithText Title { get; set; }

        [JsonProperty("callback_id")]
        public string CallbackId { get; set; }

        [JsonProperty("private_metadata")]
        public string PrivateMetadata { get; set; }
    }

    public class State
    {
        public Dictionary<string, Dictionary<string, Data>> values { get; set; }
    }

    public class Data
    {
        public string Value { get; set; }

        [JsonProperty("selected_option")]
        public SelectedOption SelectedOption { get; set; }

        [JsonProperty("selected_channel")]
        private string SelectedChannel { set { this.Value = value; } }
    }

    public class SelectedOption
    {
        public string Value { get; set; }
    }
}