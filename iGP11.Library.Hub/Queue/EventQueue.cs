using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library.Hub.Model;
using iGP11.Library.Scheduler;

namespace iGP11.Library.Hub.Queue
{
    internal sealed class EventQueue<TEvent> : ISchedulerSubscriber
        where TEvent : IEvent
    {
        private readonly IEventHandler<TEvent> _eventHandler;
        private readonly IEventRepository<TEvent> _eventRepository;
        private readonly IEventSchedulingPolicy _eventSchedulingPolicy;
        private readonly object _lock = new object();
        private readonly ILogger _logger;
        private readonly ICollection<TEvent> _scheduledEvents = new List<TEvent>();
        private readonly ConcurrentQueue<TEvent> _unprocessedEvents = new ConcurrentQueue<TEvent>();

        public EventQueue(
            IEventHandler<TEvent> eventHandler,
            IEventRepository<TEvent> eventRepository,
            IEventSchedulingPolicy eventSchedulingPolicy,
            ILogger logger)
        {
            _eventHandler = eventHandler;
            _eventRepository = eventRepository;
            _eventSchedulingPolicy = eventSchedulingPolicy;
            _logger = logger;
        }

        public void Enqueue(TEvent @event)
        {
            _unprocessedEvents.Enqueue(@event);
        }

        public bool TryScheduleTask(out IScheduledTask task)
        {
            Tick();
            TEvent @event;
            if (_unprocessedEvents.TryDequeue(out @event))
            {
                task = new EventScheduledTask(this, @event);
                return true;
            }

            task = null;
            return false;
        }

        private void LogAggregateException(AggregateException exception, TEvent @event)
        {
            var logger = new LoggerPrefixDecorator(_logger, $"event: {@event.EventId}");
            logger.Log(LogLevel.Error, "aggregate exception occured");

            foreach (var innerException in exception.Flatten().InnerExceptions)
            {
                logger.Log(LogLevel.Error, innerException.ToString());
            }
        }

        private void LogException(Exception exception, TEvent @event)
        {
            var logger = new LoggerPrefixDecorator(_logger, $"event: {@event.EventId}");
            logger.Log(LogLevel.Error, $"exception occured: {exception}");
        }

        private async Task OnFailedAsync(TEvent @event)
        {
            var time = _eventSchedulingPolicy.GetNextScheduledExecutionTime(@event.RetryCount, @event.LastExecutionTime);
            if (time.HasValue)
            {
                @event.RetryCount++;
                @event.ScheduledExecutionTime = time;
                await _eventRepository.UpdateAsync(@event);
                lock (_lock)
                {
                    _scheduledEvents.Add(@event);
                }
            }
        }

        private async Task PublishAsync(TEvent @event)
        {
            try
            {
                @event.Status = DeliveryStatus.Processing;
                await _eventRepository.UpdateAsync(@event);
                await _eventHandler.HandleAsync(@event);
                @event.Status = DeliveryStatus.Delivered;
                await _eventRepository.UpdateAsync(@event);
            }
            catch (AggregateException exception)
            {
                LogAggregateException(exception, @event);
                await OnFailedAsync(@event);
            }
            catch (Exception exception)
            {
                LogException(exception, @event);
                await OnFailedAsync(@event);
            }
        }

        private void Tick()
        {
            IEnumerable<TEvent> scheduledEvents;
            lock (_lock)
            {
                scheduledEvents = _scheduledEvents.ToArray();
            }

            foreach (var scheduledEvent in scheduledEvents)
            {
                if (scheduledEvent.ScheduledExecutionTime > DateTime.Now)
                {
                    continue;
                }

                lock (_lock)
                {
                    _scheduledEvents.Remove(scheduledEvent);
                }

                _unprocessedEvents.Enqueue(scheduledEvent);
            }
        }

        private class EventScheduledTask : IScheduledTask
        {
            private readonly TEvent _event;
            private readonly EventQueue<TEvent> _queue;

            public EventScheduledTask(EventQueue<TEvent> queue, TEvent @event)
            {
                _queue = queue;
                _event = @event;
            }

            public async Task ExecuteAsync()
            {
                await _queue.PublishAsync(_event);
            }
        }
    }
}