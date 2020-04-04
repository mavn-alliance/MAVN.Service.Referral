using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAVN.Service.Referral.MsSqlRepositories.Entities
{
    [Table("purchase_referral")]
    public class PurchaseReferralHistoryEntity : BaseEntity
    {
        [Column("referrer_id")]
        public Guid ReferrerId { get; set; }

        [Column("referred_id")]
        public Guid ReferredId { get; set; }

        public ReferralEntity Referrer { get; set; }
        public ReferralEntity Referred { get; set; }
    }
}
