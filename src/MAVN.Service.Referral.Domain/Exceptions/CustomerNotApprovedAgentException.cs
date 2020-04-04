using System;

namespace MAVN.Service.Referral.Domain.Exceptions
{
    public class CustomerNotApprovedAgentException : Exception
    {
        public CustomerNotApprovedAgentException(string message) : base(message)
        {
            
        }
    }
}