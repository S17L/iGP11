using iGP11.Library.DDD;

namespace iGP11.Tool.Domain.Model.Directory
{
    public interface IDirectoryRepository : IRepository<Directory, string>
    {
        void DemandAccess(string id);
    }
}