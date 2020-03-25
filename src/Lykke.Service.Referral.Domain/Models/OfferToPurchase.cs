using System;

namespace Lykke.Service.Referral.Domain.Models
{
    public class OfferToPurchase
    {
        public Guid Id { get; set; }

        public DateTime Timestamp { get; set; }

        public Guid ReferId { get; set; }

        public decimal? VatAmount { get; set; }

        public decimal? SellingPropertyPrice { get; set; }

        public decimal? NetPropertyPrice { get; set; }

        public decimal? DiscountAmount { get; set; }

        public string CurrencyCode { get; set; }

        public string UnitLocationCode { get; set; }
    }
}
