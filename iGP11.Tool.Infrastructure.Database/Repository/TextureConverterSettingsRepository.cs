using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Tool.Domain.Exceptions;
using iGP11.Tool.Domain.Model.TextureManagementSettings;
using iGP11.Tool.Infrastructure.Database.Model;

namespace iGP11.Tool.Infrastructure.Database.Repository
{
    public class TextureManagementSettingsRepository : ITextureManagementSettingsRepository
    {
        private readonly FileDatabaseContext _context;

        public TextureManagementSettingsRepository(FileDatabaseContext context)
        {
            _context = context;
        }

        public async Task<TextureManagementSettings> LoadAsync()
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                var model = _context.TextureConverterSettings;
                if (model == null)
                {
                    throw new AggregateRootNotFoundException("texture management settings not found");
                }

                return model.Clone();
            }
        }

        public async Task SaveAsync(TextureManagementSettings textureManagementSettings)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                _context.TextureConverterSettings = textureManagementSettings.Clone();
                _context.Commit();
            }
        }
    }
}