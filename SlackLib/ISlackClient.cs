using System.Threading.Tasks;

namespace SlackLib
{
    public interface ISlackClient
    {
        Task PostMessage(dynamic payload);
        Task OpenModelView(dynamic payload);
    }
}