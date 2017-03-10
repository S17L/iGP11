using System.Threading.Tasks;

using iGP11.Tool.ReadModel.Api.Model;

namespace iGP11.Tool.ReadModel.Api
{
    public interface IFindLastEditedGamePackageQuery
    {
        Task<GamePackage> FindAsync();
    }
}