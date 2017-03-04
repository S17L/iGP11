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

        public async Task<ApplicationSettings> LoadAsync()
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                var model = _context.ApplicationSettings;
                if (model == null)
                {
                    throw new AggregateRootNotFoundException("application settings not found");
                }

                return model.Clone();
            }
        }

        public async Task SaveAsync(ApplicationSettings applicationSettings)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                _context.ApplicationSettings = applicationSettings.Clone();
                _context.Commit();
            }
        }
    }
}