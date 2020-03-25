using System;

namespace Lykke.Service.Referral.Contract.Events
{
    /// <summary>
    /// Published when hotel referral is being used.
    /// </summary>
    public class HotelReferralUsedEvent
    {
        /// <summary>
        /// Id of the referrer Customer
        /// </summary>
        public string CustomerId { set; get; }

        /// <summary>
        /// PartnerId provided by Integration
        /// </summary>
        public string PartnerId { set; get; }

        /// <summary>
        /// LocationId provided by Integration
        /// </summary>
        public string LocationId { set; get; }

        /// <summary>
        /// The amount spent
        /// </summary>
        public decimal Amount { set; get; }

        /// <summary>
        /// The currency of the spending
        /// </summary>
        public string CurrencyCode { set; get; }

        /// <summary>
        /// Represents a Campaign id if any
        /// </summary>
        public Guid? StakedCampaignId { get; set; }

        /// <summary>
        /// Represents a referral's id
        /// </summary>
        public string ReferralId { get; set; }
    }
}
