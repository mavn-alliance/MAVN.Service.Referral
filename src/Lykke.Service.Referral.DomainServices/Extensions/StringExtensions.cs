using System.Security.Cryptography;
using System.Text;

namespace Lykke.Service.Referral.DomainServices.Extensions
{
    /// <summary>
    /// Contains common string extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Computes SHA256 hash from base64 string.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns></returns>
        public static string ToSha256Hash(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));

                var sb = new StringBuilder();

                foreach (var @byte in bytes)
                    sb.Append(@byte.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}
