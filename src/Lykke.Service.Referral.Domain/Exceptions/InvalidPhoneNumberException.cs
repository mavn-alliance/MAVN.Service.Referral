using System;

namespace Lykke.Service.Referral.Domain.Exceptions
{
    public class InvalidPhoneNumberException : Exception
    {
        public InvalidPhoneNumberException()
            : base("Invalid phone number")
        {
        }
    }
}
