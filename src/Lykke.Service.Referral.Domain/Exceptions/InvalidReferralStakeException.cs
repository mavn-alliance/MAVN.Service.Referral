using System;

namespace Lykke.Service.Referral.Domain.Exceptions
{
    public class InvalidReferralStakeException: Exception
    {
        public InvalidReferralStakeException(string message): base(message)
        {
        }
    }
}
