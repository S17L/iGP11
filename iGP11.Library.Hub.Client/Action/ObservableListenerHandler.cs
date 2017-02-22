using System.Threading.Tasks;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client.Action
{
    internal class ObservableListenerHandler<TEvent> : IListenerHandler
    {
        private readonly NotificationContext _context;
        private readonly INotificationHandler<TEvent> _handler;
        private readonly IEventSerializer<TEvent> _serializer;

        public ObservableListenerHandler(NotificationContext context, INotificationHandler<TEvent> handler, IEventSerializer<TEvent> serializer)
        {
            _context = context;
            _handler = handler;
            _serializer = serializer;
        }

        public TypeId TypeId => _serializer.TypeId;

        public async Task HandleAsync(Event @event)
        {
            _context.KeepAlive();
            await _handler.HandleAsync(_context, _serializer.Deserialize(@event));
            _context.KeepAlive();
        }
    }
}