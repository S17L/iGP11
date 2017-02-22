using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client
{
    public interface IEventSerializer<TEvent>
    {
        TypeId TypeId { get; }

        TEvent Deserialize(Event @event);

        Event Serialize(TEvent @event);
    }
}