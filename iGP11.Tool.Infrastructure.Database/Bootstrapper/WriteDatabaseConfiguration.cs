namespace iGP11.Tool.Infrastructure.Database.Bootstrapper
{
    public class WriteDatabaseConfiguration
    {
        public WriteDatabaseConfiguration(string directoryPath, string fileName, string encryptionKey)
        {
            DirectoryPath = directoryPath;
            FileName = fileName;
            EncryptionKey = encryptionKey;
        }

        public string DirectoryPath { get; private set; }

        public string EncryptionKey { get; private set; }

        public string FileName { get; private set; }
    }
}