using System.Collections.Generic;
using System.Threading.Tasks;

using iGP11.Library.DDD;

namespace iGP11.Tool.Domain.Model.InjectionSettings
{
    public interface IInjectionSettingsRepository : IRepository<InjectionSettings, AggregateId>
    {
        Task ChangeDefaultAsync(AggregateId id);

        Task<IEnumerable<InjectionSettings>> LoadAllAsync();

        Task<InjectionSettings> LoadAsync(string name);

        Task<AggregateId> LoadDefaultAsync();

        Task RemoveAsync(AggregateId id);
    }
}