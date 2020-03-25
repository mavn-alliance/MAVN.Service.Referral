using System;

namespace Lykke.Service.Referral.Domain.Exceptions
{
    public class ReferralLeadConfirmationFailedException: Exception
    {
        public ReferralLeadConfirmationFailedException(string message): base(message)
        {
            
        }
    }
}
