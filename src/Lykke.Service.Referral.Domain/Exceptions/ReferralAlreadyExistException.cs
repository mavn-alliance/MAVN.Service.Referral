using System;

namespace Lykke.Service.Referral.Domain.Exceptions
{
    public class ReferralAlreadyExistException : Exception
    {
        public ReferralAlreadyExistException()
        {
            
        }
        
        public ReferralAlreadyExistException(string message): base(message)
        {
            
        }
    }
}
