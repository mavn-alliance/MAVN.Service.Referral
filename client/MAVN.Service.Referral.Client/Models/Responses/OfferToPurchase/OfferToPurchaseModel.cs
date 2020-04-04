using System;

namespace MAVN.Service.Referral.Client.Models.Responses.OfferToPurchase
{
    /// <summary>
    /// Model representing offer to purchase model
    /// </summary>
    public class OfferToPurchaseModel
    {
        /// <summary>
        /// The id of the model
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The referral id for the offer to purchase
        /// </summary>
        public Guid ReferId { get; set; }

        /// <summary>
        /// The timestamp of the offer to purchase
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
