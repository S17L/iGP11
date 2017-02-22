using System.Threading.Tasks;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client
{
    public interface IListenerHandler
    {
        TypeId TypeId { get; }

        Task HandleAsync(Event @event);
    }
}