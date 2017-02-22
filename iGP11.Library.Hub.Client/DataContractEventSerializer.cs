using System;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client
{
    public class DataContractEventSerializer<TEvent> : IEventSerializer<TEvent>
    {
        public TypeId TypeId => TypeId.Generate<TEvent>();

        public TEvent Deserialize(Event @event)
        {
            return (TEvent)@event.Data.Deserialize(Type.GetType(@event.TypeId.Id));
        }

        public Event Serialize(TEvent @event)
        {
            return new Event(TypeId.Generate(@event.GetType()), @event.Serialize());
        }
    }
}