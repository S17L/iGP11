using System.Threading.Tasks;

using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.ReadModel.Api
{
    public interface IFindLastEditedGamePackageQuery
    {
        Task<GamePackage> FindAsync();
    }
}