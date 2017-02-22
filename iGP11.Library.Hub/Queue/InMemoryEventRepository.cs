using System.Collections.Concurrent;
using System.Threading.Tasks;

using iGP11.Library.Hub.Exceptions;
using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Queue
{
    public class InMemoryEventRepository<TEvent> : IEventRepository<TEvent>
        where TEvent : IEvent
    {
        private readonly ConcurrentDictionary<EventId, TEvent> _events = new ConcurrentDictionary<EventId, TEvent>();

        public Task<TEvent> LoadAsync(EventId id)
        {
            TEvent @event;
            if (!_events.TryGetValue(id, out @event))
            {
                throw new EventNotFoundException($"event: {id} not found");
            }

            return Task.FromResult(@event);
        }

        public async Task UpdateAsync(TEvent @event)
        {
            _events[@event.EventId] = @event;
            await Task.Yield();
        }
    }
}