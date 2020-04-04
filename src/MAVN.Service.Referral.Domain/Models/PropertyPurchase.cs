using System;

namespace MAVN.Service.Referral.Domain.Models
{
    public class PropertyPurchase
    {
        public Guid Id { get; set; }

        public Guid ReferralLeadId { get; set; }

        public DateTime Timestamp { get; set; }

        public decimal? VatAmount { get; set; }

        public decimal? SellingPropertyPrice { get; set; }

        public decimal? NetPropertyPrice { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal CalculatedCommissionAmount { get; set; }

        public string CurrencyCode { get; set; }

        public int CommissionNumber { get; set; }
    }
}
