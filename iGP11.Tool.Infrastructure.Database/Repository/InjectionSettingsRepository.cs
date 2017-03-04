using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.DDD;
using iGP11.Tool.Domain.Exceptions;
using iGP11.Tool.Domain.Model.InjectionSettings;
using iGP11.Tool.Infrastructure.Database.Model;

namespace iGP11.Tool.Infrastructure.Database.Repository
{
    public class InjectionSettingsRepository : IInjectionSettingsRepository
    {
        private readonly FileDatabaseContext _context;

        public InjectionSettingsRepository(FileDatabaseContext context)
        {
            _context = context;
        }

        public async Task ChangeDefaultAsync(AggregateId injectionSettingsId)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                if (_context.InjectionSettings.All(entity => entity.Id != injectionSettingsId))
                {
                    throw new AggregateRootNotFoundException("injection settings not found");
                }

                _context.LastEditedInjectionSettingsId = injectionSettingsId;
                _context.Commit();
            }
        }

        public async Task<IEnumerable<InjectionSettings>> LoadAllAsync()
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                return _context.InjectionSettings.Clone();
            }
        }

        public async Task<InjectionSettings> LoadAsync(AggregateId aggregateId)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                var model = _context.InjectionSettings.SingleOrDefault(entity => entity.Id == aggregateId);
                if (model == null)
                {
                    throw new AggregateRootNotFoundException("injection settings not found");
                }

                return model.Clone();
            }
        }

        public async Task<InjectionSettings> LoadAsync(string name)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                var model = _context.InjectionSettings.SingleOrDefault(entity => entity.Name == name);
                if (model == null)
                {
                    throw new AggregateRootNotFoundException("injection settings not found");
                }

                return model.Clone();
            }
        }

        public async Task<AggregateId> LoadDefaultAsync()
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                return _context.LastEditedInjectionSettingsId;
            }
        }

        public async Task RemoveAsync(AggregateId injectionSettingsId)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                _context.InjectionSettings.Remove(entity => entity.Id == injectionSettingsId);
                if (_context.LastEditedInjectionSettingsId == injectionSettingsId)
                {
                    _context.LastEditedInjectionSettingsId = _context.InjectionSettings.FirstOrDefault()?.Id;
                }

                _context.Commit();
            }
        }

        public async Task SaveAsync(InjectionSettings injectionSettings)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                _context.InjectionSettings.Remove(entity => entity.Id == injectionSettings.Id);
                _context.InjectionSettings.Add(injectionSettings.Clone());
                _context.LastEditedInjectionSettingsId = injectionSettings.Id;
                _context.Commit();
            }
        }
    }
}