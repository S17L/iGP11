using System.Linq;
using System.Threading.Tasks;

using iGP11.Library.File;
using iGP11.Tool.Application;

using DomainModel = iGP11.Tool.Domain.Model.Directory;

namespace iGP11.Tool.Infrastructure.Database.Repository
{
    public class DirectoryRepository : DomainModel.IDirectoryRepository
    {
        public void DemandAccess(string id)
        {
            new DirectoryContext(id).DemandAccess();
        }

        public Task<DomainModel.Directory> LoadAsync(string id)
        {
            var directoryContext = new DirectoryContext(id);
            directoryContext.DemandAccess();

            var files = directoryContext.GetFiles()
                .Select(file => new DomainModel.File(file.Name, new FileContentLoadingPolicy(file.Path)))
                .ToArray();

            return Task.FromResult(new DomainModel.Directory(id, files));
        }

        public async Task SaveAsync(DomainModel.Directory directory)
        {
            var directoryContext = new DirectoryContext(directory.Id);
            directoryContext.DemandAccess();

            foreach (var file in directory.GetNewFiles())
            {
                directoryContext.CreateFile(file.Name, file.Content);
            }

            directory.Commit();
            await Task.Yield();
        }
    }
}