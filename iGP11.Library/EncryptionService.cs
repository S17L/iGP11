using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace iGP11.Library
{
    public class EncryptionService : IEncryptionService
    {
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly ISecuredText _key;

        public EncryptionService(ISecuredText key)
        {
            _key = key;
        }

        public string Decrypt(string text)
        {
            var buffer = Convert.FromBase64String(text);

            using (var algorithm = CreateAlgorithm())
            using (var outputStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(outputStream, algorithm.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(buffer, 0, buffer.Length);
                cryptoStream.Close();

                return _encoding.GetString(outputStream.ToArray());
            }
        }

        public string Encrypt(string text)
        {
            var buffer = _encoding.GetBytes(text);

            using (var algorithm = CreateAlgorithm())
            using (var outputStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(outputStream, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(buffer, 0, buffer.Length);
                cryptoStream.Close();

                return Convert.ToBase64String(outputStream.ToArray());
            }
        }

        private static byte[] GetSalt()
        {
            return new byte[] { 0x53, 0x6f, 0x64, 0x69, 0x75, 0x6d, 0x20, 0x43, 0x68, 0x6c, 0x6f, 0x72, 0x69, 0x64, 0x65 };
        }

        private SymmetricAlgorithm CreateAlgorithm()
        {
            var algorithm = Rijndael.Create();
            var rdb = new Rfc2898DeriveBytes(_key.GetUnsecuredText(), GetSalt());

            algorithm.Padding = PaddingMode.ISO10126;
            algorithm.Key = rdb.GetBytes(32);
            algorithm.IV = rdb.GetBytes(16);

            return algorithm;
        }
    }
}