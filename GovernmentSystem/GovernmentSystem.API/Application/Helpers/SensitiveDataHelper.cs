using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using System.Text;

namespace GovernmentSystem.API.Application.Helpers
{
    public class SensitiveDataHelper
    {
        private readonly IDataProtector _protector;

        public SensitiveDataHelper(IDataProtectionProvider provider, IConfiguration configuration)
        {
            // Read the constant key/name configuration from appsettings.json
            string keyName = configuration["SecuritySettings:EncryptionKeyName"]
                             ?? "none";

            if (keyName == "none")
            {
                throw new InvalidOperationException("Encryption key name is not configured.");
            }

            _protector = provider.CreateProtector(keyName);
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            return _protector.Protect(plainText);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;
            return _protector.Unprotect(cipherText);
        }

        public string HashData(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Convert string to UTF-8 bytes
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // Compute SHA-256 hash securely
            byte[] hashBytes = SHA256.HashData(inputBytes);

            // Convert byte array to a lowercase hex string
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }
}
