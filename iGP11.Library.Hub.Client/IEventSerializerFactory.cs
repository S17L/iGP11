namespace iGP11.Library.Hub.Client
{
    public interface IEventSerializerFactory
    {
        IEventSerializer<TEvent> Create<TEvent>();
    }
}