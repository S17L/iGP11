using System.Threading.Tasks;

namespace iGP11.Tool.Domain.Model.TextureManagementSettings
{
    public interface ITextureManagementSettingsRepository
    {
        Task<TextureManagementSettings> LoadAsync();

        Task SaveAsync(TextureManagementSettings textureManagementSettings);
    }
}