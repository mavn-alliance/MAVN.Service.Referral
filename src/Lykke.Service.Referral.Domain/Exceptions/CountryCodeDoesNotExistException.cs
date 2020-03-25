using System;

namespace Lykke.Service.Referral.Domain.Exceptions
{
    public class CountryCodeDoesNotExistException : Exception
    {
        public CountryCodeDoesNotExistException(string message): base(message)
        {
            
        }
    }
}
