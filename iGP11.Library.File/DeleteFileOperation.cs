using System.IO;

namespace iGP11.Library.File
{
    internal class DeleteFileOperation : IOperation
    {
        private readonly string _filePath;

        private bool _isCommited;
        private string _oldContent;
        private bool _oldFileNotFound;

        public DeleteFileOperation(string filePath)
        {
            _filePath = filePath;
        }

        public void Commit()
        {
            if (System.IO.File.Exists(_filePath))
            {
                _oldContent = System.IO.File.ReadAllText(_filePath);
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
                System.IO.File.Delete(_filePath);
            }
            else
            {
                _oldFileNotFound = true;
            }

            _isCommited = true;
        }

        public void Rollback()
        {
            if (!_isCommited)
            {
                return;
            }

            if (!_oldFileNotFound)
            {
                System.IO.File.WriteAllText(_filePath, _oldContent);
            }
        }
    }
}