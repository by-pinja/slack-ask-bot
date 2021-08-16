using System.Threading.Tasks;
using SlackLib.Responses;

namespace SlackLib
{
    public interface ISlackClient
    {
        Task<ChatPostMessageResponse> PostMessage(dynamic payload);
        Task ChatUpdate(dynamic payload);
        Task OpenModelView(dynamic payload);
        Task UpdateModelView(dynamic payload);
    }
}
