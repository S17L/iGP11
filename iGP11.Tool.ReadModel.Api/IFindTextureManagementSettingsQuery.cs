using System.Threading.Tasks;

using iGP11.Tool.Shared.Model.TextureManagementSettings;

namespace iGP11.Tool.ReadModel.Api
{
    public interface IFindTextureManagementSettingsQuery
    {
        Task<TextureManagementSettings> FindAsync();
    }
}