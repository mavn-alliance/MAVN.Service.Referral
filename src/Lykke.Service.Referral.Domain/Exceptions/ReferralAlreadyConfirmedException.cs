using System;

namespace Lykke.Service.Referral.Domain.Exceptions
{
    public class ReferralAlreadyConfirmedException : Exception
    {
        public ReferralAlreadyConfirmedException() : base()
        {
            
        }
        
        public ReferralAlreadyConfirmedException(string message) : base(message)
        {

        }
    }
}