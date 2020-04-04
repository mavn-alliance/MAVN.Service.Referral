using System;

namespace MAVN.Service.Referral.Domain.Exceptions
{
    public class InvalidReferralStakeException: Exception
    {
        public InvalidReferralStakeException(string message): base(message)
        {
        }
    }
}
