using System;
using System.Threading;
using System.Windows.Threading;

namespace iGP11.Tool.Common
{
    public class TaskRunner : ITaskRunner
    {
        private static Dispatcher Dispatcher => System.Windows.Application.Current?.Dispatcher;

        public void Run(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (Dispatcher == null)
            {
                return;
            }

            if (Thread.CurrentThread.ManagedThreadId == Dispatcher.Thread.ManagedThreadId)
            {
                action();
            }
            else
            {
                Dispatcher.BeginInvoke(action, DispatcherPriority.ApplicationIdle);
            }
        }
    }
}