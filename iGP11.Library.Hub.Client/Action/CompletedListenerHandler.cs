using System.Threading.Tasks;

namespace iGP11.Library.Hub.Client.Action
{
    public class CompletedListenerHandler<TEvent> : INotificationHandler<TEvent>
    {
        private readonly INotificationHandler<TEvent> _handler;

        public CompletedListenerHandler(INotificationHandler<TEvent> handler = null)
        {
            _handler = handler;
        }

        public async Task HandleAsync(NotificationContext context, TEvent @event)
        {
            if (_handler != null)
            {
                await _handler.HandleAsync(context, @event);
            }

            await context.CompleteAsync();
        }
    }
}