using System;
using System.Threading;
using System.Threading.Tasks;

namespace iGP11.Library
{
    public class BlockingTaskQueue
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<IDisposable> GetBlockingScope()
        {
            await _semaphore.WaitAsync();
            return new BlockingScope(_semaphore);
        }

        public async void QueueAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            using (await GetBlockingScope())
            {
                action();
            }
        }

        public async void QueueTask(Func<Task> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            using (await GetBlockingScope())
            {
                await action();
            }
        }

        private class BlockingScope : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;

            public BlockingScope(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                _semaphore.Release();
            }
        }
    }
}