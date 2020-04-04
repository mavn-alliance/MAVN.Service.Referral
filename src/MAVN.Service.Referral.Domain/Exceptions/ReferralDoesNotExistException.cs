using System;

namespace MAVN.Service.Referral.Domain.Exceptions
{
    public class ReferralDoesNotExistException : Exception
    {
        public ReferralDoesNotExistException() : base()
        {
            
        }
        
        public ReferralDoesNotExistException(string message): base(message)
        {
            
        }
    }
}
