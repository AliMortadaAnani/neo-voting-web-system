using System.Security.Cryptography;
using System.Text;

namespace GovernmentSystem.API.Application.Helpers
{
    public class SensitiveDataHelper
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public SensitiveDataHelper(IConfiguration configuration)
        {
            // Read values from secrets/appsettings.json
            string keyString = configuration["SecuritySettings:AESKey"] ?? throw new InvalidOperationException("AES key not configured.");
            string ivString = configuration["SecuritySettings:AESIV"] ?? throw new InvalidOperationException("AES IV not configured.");

            // Convert to bytes (ensure correct length!)
            _key = Encoding.UTF8.GetBytes(keyString);
            _iv = Encoding.UTF8.GetBytes(ivString);

            if (_key.Length != 32) // AES-256
                throw new InvalidOperationException("AES key must be 32 bytes.");
            if (_iv.Length != 16)  // AES block size
                throw new InvalidOperationException("AES IV must be 16 bytes.");
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var encryptor = aes.CreateEncryptor();
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            return Convert.ToBase64String(cipherBytes);
        }

        public string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }

        public string HashData(string encryptedNationalId, string encryptedToken)
        {
            // Optional: handle nulls safely if inputs can be null
            encryptedNationalId ??= string.Empty;
            encryptedToken ??= string.Empty;

            // Concatenate according to your format: string1 + 10452 + string2
            string combinedInput = $"{encryptedNationalId}10452{encryptedToken}";

            // Convert the combined string to UTF-8 bytes
            byte[] inputBytes = Encoding.UTF8.GetBytes(combinedInput);

            // Compute SHA-256 hash securely
            byte[] hashBytes = SHA256.HashData(inputBytes);

            // Convert byte array to a lowercase hex string
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        private static readonly string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        // 1. National ID: NI-{Pattern}-{Suffix} (e.g., NI-JD3M1686-4X9B)
        public string GenerateNationalId(string firstName, string lastName, int governorateId, char gender, DateOnly dob)
        {
            string corePattern = BuildCorePattern(firstName, lastName, governorateId, gender, dob);
            string suffix = GenerateRandomSuffix(4);
            return $"NI-{corePattern}-{suffix}";
        }

        // 2. Token Type 1: VTK-{Pattern}-{Suffix} (e.g., VTK-JD3M1686-7Z1W)
        public string GenerateVotingToken(string firstName, string lastName, int governorateId, char gender, DateOnly dob)
        {
            string corePattern = BuildCorePattern(firstName, lastName, governorateId, gender, dob);
            string suffix = GenerateRandomSuffix(4);
            return $"VTK-{corePattern}-{suffix}";
        }

        // 3. Token Type 2: NTK-{Pattern}-{Suffix} (e.g., NTK-JD3M1686-9M2Q)
        public string GenerateNominationToken(string firstName, string lastName, int governorateId, char gender, DateOnly dob)
        {
            string corePattern = BuildCorePattern(firstName, lastName, governorateId, gender, dob);
            string suffix = GenerateRandomSuffix(4);
            return $"NTK-{corePattern}-{suffix}";
        }

        // DRY Helper Method using DateOnly
        private string BuildCorePattern(string firstName, string lastName, int governorateId, char gender, DateOnly dob)
        {
            char firstInitial = char.ToUpper(firstName.FirstOrDefault());
            char lastInitial = char.ToUpper(lastName.FirstOrDefault());
            char genderCode = char.ToUpper(gender);

            string dayFirstDigit = dob.Day.ToString()[0].ToString();
            string monthFirstDigit = dob.Month.ToString()[0].ToString();
            string shortYear = dob.ToString("yy"); // Extracts last two digits safely

            string datePart = $"{dayFirstDigit}{monthFirstDigit}{shortYear}";

            return $"{firstInitial}{lastInitial}{governorateId}{genderCode}{datePart}";
        }

        // Thread-safe random suffix generator
        private string GenerateRandomSuffix(int length)
        {
            return new string(Enumerable.Repeat(_chars, length)
                .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
        }
    }
}