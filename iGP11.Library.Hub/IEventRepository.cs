using System.Threading.Tasks;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub
{
    public interface IEventRepository<TEvent>
    {
        Task<TEvent> LoadAsync(EventId id);

        Task UpdateAsync(TEvent @event);
    }
}