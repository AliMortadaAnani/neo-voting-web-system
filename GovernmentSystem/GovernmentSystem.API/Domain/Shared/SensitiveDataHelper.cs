using Microsoft.AspNetCore.DataProtection;

namespace GovernmentSystem.API.Domain.Shared
{
    public class SensitiveDataHelper
    {
        private readonly IDataProtector _protector;

        public SensitiveDataHelper(IDataProtectionProvider provider, IConfiguration configuration)
        {
            // Read the constant key/name configuration from appsettings.json
            string keyName = configuration["SecuritySettings:EncryptionKeyName"]
                             ?? "none";

            if(keyName == "none")
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
    }
}
