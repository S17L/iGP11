namespace iGP11.Library
{
    public interface IEncryptionService
    {
        string Decrypt(string text);

        string Encrypt(string text);
    }
}