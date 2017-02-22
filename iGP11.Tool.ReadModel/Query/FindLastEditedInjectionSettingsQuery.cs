using System;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.ReadModel.Api.Exception;
using iGP11.Tool.Shared.Model.InjectionSettings;

namespace iGP11.Tool.ReadModel.Query
{
    public class FindLastEditedInjectionSettingsQuery : IFindLastEditedInjectionSettingsQuery
    {
        private readonly InMemoryDatabase _database;

        public FindLastEditedInjectionSettingsQuery(InMemoryDatabase database)
        {
            _database = database;
        }

        public Task<InjectionSettings> FindAsync(Guid id)
        {
            var model = _database.InjectionSettings.SingleOrDefault(entity => entity.Id == id);
            if (model == null)
            {
                throw new EntityNotFoundException("injection settings not found");
            }

            return Task.FromResult(model.Clone());
        }

        public Task<InjectionSettings> FindAsync()
        {
            var id = _database.LastEditedInjectionSettingsId;
            return id.HasValue
                       ? FindAsync(id.Value)
                       : Task.FromResult<InjectionSettings>(null);
        }
    }
}