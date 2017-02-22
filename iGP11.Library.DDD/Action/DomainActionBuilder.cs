using iGP11.Library.Hub.Client;
using iGP11.Library.Hub.Client.Action;
using iGP11.Library.Hub.Shared;

namespace iGP11.Library.DDD.Action
{
    public class DomainActionBuilder
    {
        private readonly ActionBuilder _actionBuilder;
        private readonly EndpointId _commandEndpointId;

        public DomainActionBuilder(ActionBuilder actionBuilder, EndpointId commandEndpointId)
        {
            _actionBuilder = actionBuilder;
            _commandEndpointId = commandEndpointId;
        }

        public ListenerBuilder<TCommand> Dispatch<TCommand>(TCommand command, IEventSerializer<Event<TCommand>> serializer = null)
        {
            var settings = new PublishSettings<Event<TCommand>>
            {
                DestinationEndpointId = _commandEndpointId,
                Serializer = serializer
            };

            return _actionBuilder.Dispatch(command, settings);
        }
    }
}