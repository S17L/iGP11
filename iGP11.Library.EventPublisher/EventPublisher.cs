using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iGP11.Library.EventPublisher
{
    public sealed class EventPublisher : IEventPublisher
    {
        private readonly IEventHandlerEqualityComparerFactory _eventHandlerEqualityComparerFactory;
        private readonly IList<WeakReference> _eventHandlers = new List<WeakReference>();
        private readonly object _lock = new object();

        public EventPublisher(IEventHandlerEqualityComparerFactory eventHandlerEqualityComparerFactory = null)
        {
            _eventHandlerEqualityComparerFactory = eventHandlerEqualityComparerFactory ?? new EventHandlerEqualityComparerFactory();
        }

        public void Collect()
        {
            lock (_lock)
            {
                foreach (var holder in from holder in _eventHandlers.ToArray()
                                       let target = holder.Target
                                       where target == null
                                       select holder)
                {
                    _eventHandlers.Remove(holder);
                }
            }
        }

        public async Task PublishAsync<TEvent>(TEvent @event)
        {
            IEnumerable<IEventHandler<TEvent>> handlers;
            lock (_lock)
            {
                handlers = GetEventHandlers<TEvent>()
                    .ToArray();
            }

            foreach (var eventHandler in handlers)
            {
                await eventHandler.HandleAsync(@event);
            }
        }

        public void Register<TObject>(IEventHandler<TObject> eventHandler)
        {
            lock (_lock)
            {
                var comparer = _eventHandlerEqualityComparerFactory.Create<TObject>();
                if (!GetEventHandlers<TObject>()
                        .Any(handler => comparer.Equals(handler, eventHandler)))
                {
                    _eventHandlers.Add(new WeakReference(eventHandler));
                }
            }
        }

        public void Unregister<TObject>(IEventHandler<TObject> eventHandler)
        {
            lock (_lock)
            {
                var comparer = _eventHandlerEqualityComparerFactory.Create<TObject>();
                foreach (var holder in from holder in _eventHandlers.ToArray()
                                       let handler = holder.Target as IEventHandler<TObject>
                                       where (handler != null) && comparer.Equals(handler, eventHandler)
                                       select holder)
                {
                    _eventHandlers.Remove(holder);
                }
            }
        }

        private IEnumerable<IEventHandler<TObject>> GetEventHandlers<TObject>()
        {
            return _eventHandlers.Select(holder => holder.Target)
                .OfType<IEventHandler<TObject>>()
                .ToArray();
        }
    }
}