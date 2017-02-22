using System.Threading.Tasks;

namespace iGP11.Library.Hub.Client.Action
{
    public interface ITimeoutHandler
    {
        Task HandleAsync();
    }
}