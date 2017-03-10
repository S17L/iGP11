using System;
using System.Threading.Tasks;

using iGP11.Library;

namespace iGP11.Tool.ReadModel
{
    internal static class IsolatedDatabaseAccess
    {
        private static readonly BlockingTaskQueue _queue = new BlockingTaskQueue();

        public static Task<IDisposable> Open()
        {
            return _queue.GetBlockingScope();
        }
    }
}