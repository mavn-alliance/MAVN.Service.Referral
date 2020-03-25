using System;

namespace Lykke.Service.Referral.Domain.Models
{
    public class Referral
    {
        public string Id { get; set; }

        public string ReferralCode { get; set; }

        public Guid CustomerId { get; set; }
    }
}
