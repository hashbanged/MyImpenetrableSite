using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MyImpenetrableSite
{
    public class Hasher
    {
        // Refer to: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-5.0
        public static string Hash(string inputText)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: inputText,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return Convert.ToBase64String(salt) + ":" + hashed;
        }

        // Refer to: https://stackoverflow.com/questions/45220359/encrypting-and-verifying-a-hashed-password-with-salt-using-pbkdf2-encryption
        public static bool Verify(string plainText, string pwSaltAndHash)
        {
            if (!pwSaltAndHash.Contains(":"))
            {
                return false;
            }

            string[] parts = pwSaltAndHash.Split(':');
            string pwSalt = parts[0];
            string pwHash = parts[1];

            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: plainText,
                salt: Convert.FromBase64String(pwSalt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return pwHash == hashedPassword;
        }
    }
}
