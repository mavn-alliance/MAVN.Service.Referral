using System;

namespace Lykke.Service.Referral.Domain.Models
{
    public class ReferralFriend
    {
        public Guid Id { get; set; }
        
        public string Email { get; set; }

        public string FullName { get; set; }

        public Guid ReferrerId { get; set; }

        public Guid ReferredId { get; set; }

        public Guid CampaignId { get; set; }

        public ReferralFriendState State { get; set; }

        public DateTime CreationDateTime { get; set; }
    }
}
