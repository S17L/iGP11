using System;
using System.Threading.Tasks;

namespace iGP11.Library.Hub.Client.Action
{
    public class LambdaNotificationHandler<TEvent> : INotificationHandler<TEvent>
    {
        private readonly Action<NotificationContext, TEvent> _lambda;

        public LambdaNotificationHandler(Action<NotificationContext, TEvent> lambda)
        {
            _lambda = lambda;
        }

        public async Task HandleAsync(NotificationContext context, TEvent @event)
        {
            _lambda(context, @event);
            await Task.Yield();
        }
    }
}