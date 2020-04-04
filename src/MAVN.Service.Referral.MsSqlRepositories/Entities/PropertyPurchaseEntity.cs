using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAVN.Service.Referral.MsSqlRepositories.Entities
{
    [Table("property_purchase")]
    public class PropertyPurchaseEntity : BaseEntity
    {
        [Column("refer_lead_id")]
        public Guid ReferralLeadId { get; set; }

        [Column("commission_number")]
        public int CommissionNumber { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
