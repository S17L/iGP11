using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iGP11.Library.Scheduler
{
    public sealed class AsynchronousScheduler : IAsynchronousScheduler
    {
        private const int Interval = 50;
        private const int TickInterval = 50;

        private readonly object _lock = new object();
        private readonly ILogger _logger;
        private readonly int _maxCount;
        private readonly ICollection<ISchedulerSubscriber> _subscribers = new List<ISchedulerSubscriber>();

        public AsynchronousScheduler(ILogger logger, int maxCount = 10)
        {
            _logger = logger;
            _maxCount = maxCount;
        }

        public bool IsRunning { get; private set; }

        public void Dispose()
        {
            Stop();
        }

        public async void Start()
        {
            await StartAsync();
        }

        public async Task StartAsync(bool exitOnEmpty = false)
        {
            if (IsRunning)
            {
                return;
            }

            IsRunning = true;
            while (IsRunning)
            {
                var tasks = new List<Task>();
                try
                {
                    using (var enumerator = GetScheduledTasksEnumerator())
                    {
                        do
                        {
                            try
                            {
                                while (IsRunning && (tasks.Count < _maxCount) && enumerator.MoveNext())
                                {
                                    tasks.Add(enumerator.Current.ExecuteAsync());
                                }

                                await RunAsync(tasks);
                            }
                            catch (Exception exception)
                            {
                                _logger.Log(LogLevel.Error, $"inner loop exception; exception: {exception}");
                            }
                        }
                        while (tasks.Any());
                    }
                }
                catch (Exception exception)
                {
                    _logger.Log(LogLevel.Error, $"main loop exception; exception: {exception}");
                }

                if (!IsRunning)
                {
                    break;
                }

                if (exitOnEmpty)
                {
                    Stop();
                    break;
                }

                await Task.Delay(Interval);
            }
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Subscribe(ISchedulerSubscriber subscriber)
        {
            lock (_lock)
            {
                _subscribers.Add(subscriber);
            }
        }

        public void Unsubscribe(ISchedulerSubscriber subscriber)
        {
            lock (_lock)
            {
                _subscribers.Remove(subscriber);
            }
        }

        private IEnumerator<IScheduledTask> GetScheduledTasksEnumerator()
        {
            var isRunning = true;
            while (isRunning)
            {
                IEnumerable<ISchedulerSubscriber> subscribers;
                lock (_lock)
                {
                    subscribers = _subscribers.ToArray();
                }

                isRunning = false;
                foreach (var subscriber in subscribers)
                {
                    IScheduledTask task;
                    if (!subscriber.TryScheduleTask(out task))
                    {
                        continue;
                    }

                    isRunning = true;
                    yield return task;
                }
            }
        }

        private void HandleException(Exception exception)
        {
            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                var logger = new LoggerPrefixDecorator(_logger, "aggregate exception");
                logger.Log(LogLevel.Error, "aggregate exception occured");

                foreach (var innerException in aggregateException.Flatten().InnerExceptions)
                {
                    logger.Log(LogLevel.Error, innerException.ToString());
                }
            }
            else
            {
                var logger = new LoggerPrefixDecorator(_logger, "exception");
                logger.Log(LogLevel.Error, exception.ToString());
            }
        }

        private async Task RunAsync(ICollection<Task> tasks)
        {
            var tick = Task.Delay(TickInterval);
            tasks.Add(tick);
            var completedTask = await Task.WhenAny(tasks);
            if ((completedTask.Status == TaskStatus.Canceled)
                || (completedTask.Status == TaskStatus.Faulted)
                || (completedTask.Status == TaskStatus.RanToCompletion))
            {
                if ((completedTask.Status == TaskStatus.Faulted) && (completedTask.Exception != null))
                {
                    HandleException(completedTask.Exception);
                }

                tasks.Remove(completedTask);
                tasks.Remove(tick);
            }
        }
    }
}