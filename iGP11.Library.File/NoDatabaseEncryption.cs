namespace iGP11.Library.File
{
    public class NoDatabaseEncryption : IDatabaseEncryption
    {
        public string Decrypt(string database)
        {
            return database;
        }

        public string Encrypt(string database)
        {
            return database;
        }
    }
}