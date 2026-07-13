using System;
using System.Security.Cryptography;
using System.Text;

namespace Server.Services
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (hashedPassword == null) throw new ArgumentNullException(nameof(hashedPassword));

            var hash = HashPassword(password);
            return string.Equals(hash, hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
