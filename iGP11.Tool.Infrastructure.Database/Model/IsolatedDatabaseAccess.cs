using System;
using System.Threading.Tasks;

using iGP11.Library;

namespace iGP11.Tool.Infrastructure.Database.Model
{
    internal static class IsolatedDatabaseAccess
    {
        private static readonly BlockingTaskQueue _queue = new BlockingTaskQueue();

        public static async Task<IDisposable> Open()
        {
            return await _queue.GetBlockingScope();
        }
    }
}