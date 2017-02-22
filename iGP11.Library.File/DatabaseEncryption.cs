namespace iGP11.Library.File
{
    public class DatabaseEncryption : IDatabaseEncryption
    {
        private readonly IEncryptionService _encryptionService;

        public DatabaseEncryption(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public string Decrypt(string database)
        {
            return _encryptionService.Decrypt(database);
        }

        public string Encrypt(string database)
        {
            return _encryptionService.Encrypt(database);
        }
    }
}