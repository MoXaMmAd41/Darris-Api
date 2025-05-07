using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Darris_Api.PasswordHashed
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            byte[] salt=new byte[128/8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string Hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password = password,
                salt = salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested : 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{Hashed}";
        }
        public static bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            var parts = storedPassword.Split('.');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = parts[1];

            string hashedInput = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashedInput == storedHash;
        }
    }
}
