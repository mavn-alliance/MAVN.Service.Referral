using System;

namespace MAVN.Service.Referral.Domain.Exceptions
{
    public class ReferralLeadConfirmationFailedException: Exception
    {
        public ReferralLeadConfirmationFailedException(string message): base(message)
        {
            
        }
    }
}
