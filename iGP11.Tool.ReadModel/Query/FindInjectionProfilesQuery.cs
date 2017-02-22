using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.ReadModel.Api.Model;

namespace iGP11.Tool.ReadModel.Query
{
    public class FindInjectionProfilesQuery : IFindInjectionProfilesQuery
    {
        private readonly InMemoryDatabase _database;

        public FindInjectionProfilesQuery(InMemoryDatabase database)
        {
            _database = database;
        }

        public Task<IEnumerable<InjectionProfile>> FindAllAsync()
        {
            var profiles = _database.InjectionSettings
                .Select(settings => new InjectionProfile(settings.Id, settings.Name))
                .OrderBy(profile => profile.Name)
                .ToArray();

            return Task.FromResult<IEnumerable<InjectionProfile>>(profiles);
        }
    }
}