using System;

namespace Lykke.Service.Referral.Domain.Exceptions
{
    public class ReferralAlreadyUsedException : Exception
    {
        public ReferralAlreadyUsedException(string referralId)
        {
            ReferralId = referralId;
        }
        
        public string ReferralId { get; }
    }
}