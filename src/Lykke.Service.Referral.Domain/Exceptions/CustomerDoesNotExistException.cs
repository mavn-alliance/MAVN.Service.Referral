using System;

namespace Lykke.Service.Referral.Domain.Exceptions
{
    public class CustomerDoesNotExistException : Exception
    {
        public CustomerDoesNotExistException() : base()
        {
            
        }
        
        public CustomerDoesNotExistException(string message) : base(message)
        {
            
        }
    }
}