using System;

namespace Lykke.Service.Referral.Domain.Exceptions
{
    public class PartnerNotFoundException: Exception
    {
        public PartnerNotFoundException(string message): base(message)
        {
        }
    }
}
