using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lykke.Service.Referral.MsSqlRepositories.Entities
{
    [Table("offer_to_purchase")]
    public class OfferToPurchaseEntity : BaseEntity
    {
        [Column("refer_id")]
        public Guid ReferId { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
