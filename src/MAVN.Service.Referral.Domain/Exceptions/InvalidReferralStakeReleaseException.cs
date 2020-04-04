using System;

namespace MAVN.Service.Referral.Domain.Exceptions
{
    public class InvalidReferralStakeReleaseException : Exception
    {
        public InvalidReferralStakeReleaseException(string message): base(message)
        {
        }
    }
}
