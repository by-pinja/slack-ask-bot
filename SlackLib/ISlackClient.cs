using System.Threading.Tasks;
using SlackLib.Requests;
using SlackLib.Responses;

namespace SlackLib
{
    public interface ISlackClient
    {
        Task<ChatPostMessageResponse> PostMessage(ChatPostMessageRequest payload);
        Task ChatUpdate(dynamic payload);
        Task OpenModelView(dynamic payload);
        Task UpdateModelView(dynamic payload);
    }
}
