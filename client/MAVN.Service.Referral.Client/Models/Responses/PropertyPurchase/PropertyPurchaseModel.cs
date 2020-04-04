using System;

namespace MAVN.Service.Referral.Client.Models.Responses.PropertyPurchase
{
    /// <summary>
    /// Model representing property purchase model
    /// </summary>
    public class PropertyPurchaseModel
    {
        /// <summary>
        /// The id of the model
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The referral id for the property purchase
        /// </summary>
        public string ReferralLeadId { get; set; }

        /// <summary>
        /// The timestamp of the property purchase
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
