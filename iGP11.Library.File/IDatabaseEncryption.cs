namespace iGP11.Library.File
{
    public interface IDatabaseEncryption
    {
        string Decrypt(string database);

        string Encrypt(string database);
    }
}