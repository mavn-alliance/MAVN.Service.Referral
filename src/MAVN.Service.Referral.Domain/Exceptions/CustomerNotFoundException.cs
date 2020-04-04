using System;

namespace MAVN.Service.Referral.Domain.Exceptions
{
    public class CustomerNotFoundException: Exception
    {
        public CustomerNotFoundException(string message): base(message)
        {
            
        }
    }
}
