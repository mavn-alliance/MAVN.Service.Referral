using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.Referral.Domain.Managers;

namespace MAVN.Service.Referral.DomainServices.Managers
{
    public class HashingManager: IHashingManager
    {
        private readonly ILog _log;

        public HashingManager(ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
        }

        public string GenerateBase(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                if (string.IsNullOrEmpty(input))
                {
                    var exception = new ArgumentNullException(nameof(input));
                    _log.Error("Input parameter is null.", exception);
                    throw exception;
                }
                var bytes = Encoding.UTF8.GetBytes(input);
                var sha = sha256.ComputeHash(bytes);

                return GetBase34String(sha);
            }
        }

        private string GetBase34String(byte[] toConvert)
        {
            const string alphabet = "123456789abcdefghijklmnpqrstuvwxyz";
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(toConvert);
            } 

            var dividend = new BigInteger(toConvert);
            var builder = new StringBuilder();

            while (dividend != 0)
            {
                dividend = BigInteger.DivRem(dividend, alphabet.Length, out var remainder);
                builder.Insert(0, alphabet[Math.Abs(((int)remainder))]);
            }

            return builder.ToString();
        }
    }
}
