using System;

namespace Lykke.Service.Referral.Contract.Events
{
    /// <summary>
    /// Represents an event sent when property is purchased
    /// </summary>
    public class PropertyPurchaseReferralEvent
    {
        /// <summary>
        /// Represents the Id of the referrer
        /// </summary>
        public string ReferrerId { get; set; }

        /// <summary>
        /// Represents a timestamp of the referral
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Represents a Vat Amount
        /// </summary>
        public decimal? VatAmount { get; set; }

        /// <summary>
        /// Represents a Selling Property Price
        /// </summary>
        public decimal? SellingPropertyPrice { get; set; }

        /// <summary>
        /// Represents a Net Property Price
        /// </summary>
        public decimal? NetPropertyPrice { get; set; }

        /// <summary>
        /// Represents a Discount Amount
        /// </summary>
        public decimal? DiscountAmount { get; set; }

        /// <summary>
        /// Represents a Calculated Commission Amount
        /// </summary>
        public decimal CalculatedCommissionAmount { get; set; }

        /// <summary>
        /// Represents a Currency Code of the amounts
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Represents a referral's id
        /// </summary>
        public string ReferralId { get; set; }
    }
}
