using iGP11.Tool.Domain.Model.Directory;

using File = System.IO.File;

namespace iGP11.Tool.Application
{
    public class FileContentLoadingPolicy : IFileContentLoadingPolicy
    {
        private readonly string _filePath;

        public FileContentLoadingPolicy(string filePath)
        {
            _filePath = filePath;
        }

        public byte[] Load()
        {
            return File.ReadAllBytes(_filePath);
        }
    }
}