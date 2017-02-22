using System;
using System.Threading.Tasks;

namespace iGP11.Library.Hub.Client.Action
{
    public class AsynchronousLambdaNotificationHandler<TEvent> : INotificationHandler<TEvent>
    {
        private readonly Func<NotificationContext, TEvent, Task> _lambda;

        public AsynchronousLambdaNotificationHandler(Func<NotificationContext, TEvent, Task> lambda)
        {
            _lambda = lambda;
        }

        public async Task HandleAsync(NotificationContext context, TEvent @event)
        {
            await _lambda(context, @event);
        }
    }
}