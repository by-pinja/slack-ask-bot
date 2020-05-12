using System.Threading.Tasks;

namespace SlackLib
{
    interface ISlackClient
    {
        Task PostMessage(dynamic payload);
        Task OpenModelView(dynamic payload);
        Task UpdateModelView(dynamic payload);
    }
}