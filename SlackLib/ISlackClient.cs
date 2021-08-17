using System.Threading.Tasks;
using SlackLib.Requests;
using SlackLib.Responses;

namespace SlackLib
{
    public interface ISlackClient
    {
        Task<ChatPostMessageResponse> PostMessage(ChatPostMessageRequest payload);
        Task ChatUpdate(ChatUpdateRequest payload);
        Task OpenModelView(ViewsOpenRequest payload);
        Task UpdateModelView(ViewsUpdateRequest payload);
    }
}
