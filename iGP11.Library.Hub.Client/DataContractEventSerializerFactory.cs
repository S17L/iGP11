namespace iGP11.Library.Hub.Client
{
    public class DataContractEventSerializerFactory : IEventSerializerFactory
    {
        public IEventSerializer<TEvent> Create<TEvent>()
        {
            return new DataContractEventSerializer<TEvent>();
        }
    }
}