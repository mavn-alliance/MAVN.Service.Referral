using System;

namespace MAVN.Service.Referral.Domain.Exceptions
{
    public class ReferralExpiredException : Exception
    {
        public ReferralExpiredException(string referralId)
        {
            ReferralId = referralId;
        }
        
        public string ReferralId { get; }
    }
}