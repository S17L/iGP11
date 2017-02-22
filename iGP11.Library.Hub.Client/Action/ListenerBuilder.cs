using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client.Action
{
    public class ListenerBuilder<TAction>
    {
        private readonly TAction _action;
        private readonly NotificationContext _context = new NotificationContext();
        private readonly IEventSerializerFactory _defaultSerializerFactory;
        private readonly ICollection<object> _handlers = new List<object>();
        private readonly IHubClientFactory _hubClientFactory;
        private readonly PublishSettings<Event<TAction>> _publishSettings;

        private ITimeoutHandler _timeoutHandler = new ThrowExceptionTimeoutHandler();

        public ListenerBuilder(
            TAction action,
            PublishSettings<Event<TAction>> publishSettings,
            IHubClientFactory hubClientFactory,
            IEventSerializerFactory defaultSerializerFactory)
        {
            _action = action;
            _publishSettings = publishSettings;
            _hubClientFactory = hubClientFactory;
            _defaultSerializerFactory = defaultSerializerFactory;
        }

        public ListenerBuilder<TAction> CompleteFor<TEvent>(INotificationHandler<TEvent> handler, IEventSerializer<TEvent> serializer = null)
        {
            ListenFor(new CompletedListenerHandler<TEvent>(handler), serializer);
            return this;
        }

        public ListenerBuilder<TAction> CompleteFor<TEvent>(Func<NotificationContext, TEvent, Task> handler, IEventSerializer<TEvent> serializer = null)
        {
            ListenFor(new CompletedListenerHandler<TEvent>(new AsynchronousLambdaNotificationHandler<TEvent>(handler)), serializer);
            return this;
        }

        public ListenerBuilder<TAction> CompleteFor<TEvent>(Action<NotificationContext, TEvent> handler, IEventSerializer<TEvent> serializer = null)
        {
            ListenFor(new CompletedListenerHandler<TEvent>(new LambdaNotificationHandler<TEvent>(handler)), serializer);
            return this;
        }

        public ListenerBuilder<TAction> CompleteFor<TEvent>(IEventSerializer<TEvent> serializer = null)
        {
            ListenFor(new CompletedListenerHandler<TEvent>(), serializer);
            return this;
        }

        public async Task Execute(Action<ExecutionConfiguration> configurator = null)
        {
            var id = EndpointId.Generate();
            var configuration = new ExecutionConfiguration();
            configurator?.Invoke(configuration);

            using (var hubClient = _hubClientFactory.Create(id))
            {
                var listeners = new List<IListener>();
                try
                {
                    listeners.AddRange((from dynamic handler in _handlers
                                        select hubClient.ListenFor(handler)).Cast<IListener>());

                    var startTime = DateTime.Now;
                    _context.KeepAlive();
                    await hubClient.PublishAsync(new Event<TAction>(_action, id), _publishSettings);
                    _context.KeepAlive();

                    while (listeners.Any() && !_context.Handled)
                    {
                        var currentTime = DateTime.Now;
                        if ((startTime.Add(configuration.ActionTimeout) < currentTime)
                            || (_context.Usage.GetValueOrDefault().Add(configuration.EventTimeout) < currentTime))
                        {
                            if (_timeoutHandler != null)
                            {
                                await _timeoutHandler.HandleAsync();
                            }

                            break;
                        }

                        await Task.Delay(configuration.Tick);
                    }
                }
                finally
                {
                    foreach (var listener in listeners)
                    {
                        listener.Dispose();
                    }
                }
            }
        }

        public ListenerBuilder<TAction> ListenFor<TEvent>(INotificationHandler<TEvent> handler, IEventSerializer<TEvent> serializer = null)
        {
            _handlers.Add(new ObservableListenerHandler<TEvent>(
                _context,
                handler,
                serializer ?? _defaultSerializerFactory.Create<TEvent>()));

            return this;
        }

        public ListenerBuilder<TAction> ListenFor<TEvent>(Func<NotificationContext, TEvent, Task> handler, IEventSerializer<TEvent> serializer = null)
        {
            ListenFor(new AsynchronousLambdaNotificationHandler<TEvent>(handler), serializer);
            return this;
        }

        public ListenerBuilder<TAction> ListenFor<TEvent>(Action<NotificationContext, TEvent> handler, IEventSerializer<TEvent> serializer = null)
        {
            ListenFor(new LambdaNotificationHandler<TEvent>(handler), serializer);
            return this;
        }

        public ListenerBuilder<TAction> OnTimeout(ITimeoutHandler handler)
        {
            _timeoutHandler = handler;
            return this;
        }

        public ListenerBuilder<TAction> OnTimeout(Func<Task> action)
        {
            _timeoutHandler = new AsynchronousLambdaTimeoutHandler(action);
            return this;
        }

        public ListenerBuilder<TAction> OnTimeout(System.Action action)
        {
            _timeoutHandler = new LambdaTimeoutHandler(action);
            return this;
        }

        public ListenerBuilder<TAction> OnTimeout()
        {
            _timeoutHandler = null;
            return this;
        }
    }
}