using System;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.ReadModel.Api.Exception;
using iGP11.Tool.Shared.Model.InjectionSettings;

namespace iGP11.Tool.ReadModel.Query
{
    public class FindInjectionSettingsByIdQuery : IFindInjectionSettingsByIdQuery
    {
        private readonly InMemoryDatabase _database;

        public FindInjectionSettingsByIdQuery(InMemoryDatabase database)
        {
            _database = database;
        }

        public Task<InjectionSettings> FindByIdAsync(Guid id)
        {
            var model = _database.InjectionSettings.SingleOrDefault(entity => entity.Id == id);
            if (model == null)
            {
                throw new EntityNotFoundException("injection settings not found");
            }

            return Task.FromResult(model.Clone());
        }
    }
}