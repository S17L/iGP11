using System.IO;

namespace iGP11.Library.File
{
    internal class CreateFileOperation : IOperation
    {
        private readonly string _filePath;
        private readonly byte[] _newContent;

        private bool _isCommited;
        private byte[] _oldContent;
        private bool _oldFileNotFound;

        public CreateFileOperation(string filePath, byte[] newContent)
        {
            _filePath = filePath;
            _newContent = newContent;
        }

        public void Commit()
        {
            if (System.IO.File.Exists(_filePath))
            {
                _oldContent = System.IO.File.ReadAllBytes(_filePath);
            }
            else
            {
                _oldFileNotFound = true;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(_filePath) ?? string.Empty);
            System.IO.File.WriteAllBytes(_filePath, _newContent);
            _isCommited = true;
        }

        public void Rollback()
        {
            if (!_isCommited)
            {
                return;
            }

            if (_oldFileNotFound)
            {
                System.IO.File.Delete(_filePath);
            }
            else
            {
                System.IO.File.WriteAllBytes(_filePath, _oldContent);
            }
        }
    }
}