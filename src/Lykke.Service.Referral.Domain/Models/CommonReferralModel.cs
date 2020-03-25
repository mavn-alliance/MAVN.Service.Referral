using System;

namespace Lykke.Service.Referral.Domain.Models
{
    public class CommonReferralModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime TimeStamp { get; set; }

        public CommonReferralStatus Status { get; set; }

        public ReferralType ReferralType { get; set; }

        public Guid? CampaignId { get; set; }

        public Guid? PartnerId { get; set; }
    }
}
