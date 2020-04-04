using System;

namespace MAVN.Service.Referral.Domain.Exceptions
{
    public class ReferralNotConfirmedException : Exception
    {
        public ReferralNotConfirmedException(string referralId)
        {
            ReferralId = referralId;
        }
        
        public string ReferralId { get; }
    }
}