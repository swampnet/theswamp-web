using System.Threading.Tasks;
using TheSwamp.Shared;

namespace TheSwamp.Api.Interfaces
{
    public interface IPostMessage
    {
        Task PostAsync(AgentMessage msg);
    }
}
