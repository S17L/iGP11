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
            if (_context.InjectionSettings.All(entity => entity.Id != injectionSettingsId))
            {
                throw new AggregateRootNotFoundException("injection settings not found");
            }

            _context.LastEditedInjectionSettingsId = injectionSettingsId;
            _context.Commit();

            await Task.Yield();
        }

        public Task<IEnumerable<InjectionSettings>> LoadAllAsync()
        {
            return Task.FromResult<IEnumerable<InjectionSettings>>(_context.InjectionSettings.Clone());
        }

        public Task<InjectionSettings> LoadAsync(AggregateId aggregateId)
        {
            var model = _context.InjectionSettings.SingleOrDefault(entity => entity.Id == aggregateId);
            if (model == null)
            {
                throw new AggregateRootNotFoundException("injection settings not found");
            }

            return Task.FromResult(model.Clone());
        }

        public Task<InjectionSettings> LoadAsync(string name)
        {
            var model = _context.InjectionSettings.SingleOrDefault(entity => entity.Name == name);
            if (model == null)
            {
                throw new AggregateRootNotFoundException("injection settings not found");
            }

            return Task.FromResult(model.Clone());
        }

        public Task<AggregateId> LoadDefaultAsync()
        {
            return Task.FromResult(_context.LastEditedInjectionSettingsId);
        }

        public async Task RemoveAsync(AggregateId injectionSettingsId)
        {
            _context.InjectionSettings.Remove(entity => entity.Id == injectionSettingsId);

            if (_context.LastEditedInjectionSettingsId == injectionSettingsId)
            {
                _context.LastEditedInjectionSettingsId = _context.InjectionSettings.FirstOrDefault()
                    ?.Id;
            }

            _context.Commit();
            await Task.Yield();
        }

        public async Task SaveAsync(InjectionSettings injectionSettings)
        {
            _context.InjectionSettings.Remove(entity => entity.Id == injectionSettings.Id);
            _context.InjectionSettings.Add(injectionSettings.Clone());
            _context.LastEditedInjectionSettingsId = injectionSettings.Id;
            _context.Commit();

            await Task.Yield();
        }
    }
}