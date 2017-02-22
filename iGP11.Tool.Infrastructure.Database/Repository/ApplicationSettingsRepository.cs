using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Tool.Domain.Exceptions;
using iGP11.Tool.Domain.Model.ApplicationSettings;
using iGP11.Tool.Infrastructure.Database.Model;

namespace iGP11.Tool.Infrastructure.Database.Repository
{
    public class ApplicationSettingsRepository : IApplicationSettingsRepository
    {
        private readonly FileDatabaseContext _context;

        public ApplicationSettingsRepository(FileDatabaseContext context)
        {
            _context = context;
        }

        public Task<ApplicationSettings> LoadAsync()
        {
            var model = _context.ApplicationSettings;
            if (model == null)
            {
                throw new AggregateRootNotFoundException("application settings not found");
            }

            return Task.FromResult(model.Clone());
        }

        public async Task SaveAsync(ApplicationSettings applicationSettings)
        {
            _context.ApplicationSettings = applicationSettings.Clone();
            _context.Commit();

            await Task.Yield();
        }
    }
}