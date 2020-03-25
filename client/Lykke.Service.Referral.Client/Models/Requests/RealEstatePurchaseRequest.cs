using System;

namespace Lykke.Service.Referral.Client.Models.Requests
{
    /// <summary>
    /// A model representing real estate creation request
    /// </summary>
    public class RealEstatePurchaseRequest
    {
        /// <summary>
        /// The id of the lead referral for the real estate purchase.
        /// </summary>
        public string ReferralId { get; set; }

        /// <summary>
        /// The timestamp of the purchase
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
