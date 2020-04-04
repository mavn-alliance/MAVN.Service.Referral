using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAVN.Service.Referral.MsSqlRepositories.Entities
{
    [Table("friend_referral")]
    public class FriendReferralEntity : BaseEntity
    {
        [Column("referrer_id")]
        public Guid ReferrerId { get; set; }

        [Column("referred_id")]
        public Guid? ReferredId { get; set; }

        [Column("campaign_id")]
        public Guid CampaignId { get; set; }

        [Column("state")]
        public ReferralFriendState State { get; set; }

        [Column("creation_datetime")]
        public DateTime CreationDateTime { get; set; }
    }
}
