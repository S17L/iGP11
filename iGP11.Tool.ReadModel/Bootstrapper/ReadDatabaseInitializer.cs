using System.Threading;
using System.Threading.Tasks;

namespace iGP11.Tool.ReadModel.Bootstrapper
{
    public static class ReadDatabaseInitializer
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public static InMemoryDatabase Database { get; private set; }

        public static async Task Initialize()
        {
            await _semaphore.WaitAsync();

            try
            {
                Database = new InMemoryDatabase
                {
                    ConstantSettings = ConstantSettingsProvider.Find()
                };
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}