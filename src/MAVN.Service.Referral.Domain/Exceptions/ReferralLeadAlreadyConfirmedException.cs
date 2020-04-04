using System;

namespace MAVN.Service.Referral.Domain.Exceptions
{
    public class ReferralLeadAlreadyConfirmedException : Exception
    {
        public ReferralLeadAlreadyConfirmedException(string message)
        : base(message)
        {

        }
    }
}
