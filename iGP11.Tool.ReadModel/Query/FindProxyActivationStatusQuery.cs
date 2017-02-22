using System.Threading.Tasks;

using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ReadModel.Query
{
    public class FindProxyActivationStatusQuery : IFindProxyActivationStatusQuery
    {
        private readonly InMemoryDatabase _database;

        public FindProxyActivationStatusQuery(InMemoryDatabase database)
        {
            _database = database;
        }

        public Task<ActivationStatus> FindActivationStatusAsync(string applicationFilePath)
        {
            ActivationStatus status;
            status = _database.ProxyActivationStatuses.TryGetValue(applicationFilePath, out status)
                         ? status
                         : ActivationStatus.NotRetrievable;

            return Task.FromResult(status);
        }
    }
}