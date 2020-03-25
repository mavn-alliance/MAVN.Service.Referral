using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lykke.Service.Referral.MsSqlRepositories.Entities
{
    [Table("customer_referral")]
    public class ReferralEntity
    {
        [Key]
        [Column("customer_id")]
        public Guid CustomerId { get; set; }

        [Column("referral_code", TypeName = "varchar(64)")]
        public string ReferralCode { get; set; }

        public ICollection<PurchaseReferralHistoryEntity> PurchaseReferrers { get; set; }
        public ICollection<PurchaseReferralHistoryEntity> PurchasesReferred { get; set; }
    }
}
