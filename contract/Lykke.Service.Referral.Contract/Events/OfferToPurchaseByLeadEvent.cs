using System;
using JetBrains.Annotations;

namespace Lykke.Service.Referral.Contract.Events
{
    /// <summary>
    /// Represents an event sent when offer to purchase is made
    /// </summary>
    [PublicAPI]
    public class OfferToPurchaseByLeadEvent
    {
        /// <summary>
        /// Represents the Id of the agent customer
        /// </summary>
        public string AgentId { get; set; }

        /// <summary>
        /// Represents a timestamp of the offer
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
        /// Represents a Currency Code of the amounts
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// The Campaign id of the referral
        /// </summary>
        public Guid CampaignId { get; set; }

        /// <summary>Unit location code</summary>
        public string UnitLocationCode { get; set; }

        /// <summary>
        /// Represents a referral's id
        /// </summary>
        public string ReferralId { get; set; }
    }
}
