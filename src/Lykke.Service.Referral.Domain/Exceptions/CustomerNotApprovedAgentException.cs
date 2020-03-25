using System;

namespace Lykke.Service.Referral.Domain.Exceptions
{
    public class CustomerNotApprovedAgentException : Exception
    {
        public CustomerNotApprovedAgentException(string message) : base(message)
        {
            
        }
    }
}