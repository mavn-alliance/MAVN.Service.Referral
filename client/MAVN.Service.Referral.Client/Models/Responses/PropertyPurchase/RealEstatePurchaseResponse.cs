using System;
using MAVN.Service.Referral.Client.Enums;

namespace MAVN.Service.Referral.Client.Models.Responses.PropertyPurchase
{
    /// <summary>
    /// A model representing the real estate purchase creation result
    /// </summary>
    public class RealEstatePurchaseResponse
    {
        /// <summary>
        /// The id ot the campaign
        /// </summary>
        public Guid CampaignId { get; set; }

        /// <summary>
        /// The error code that indicates the success of the operation.
        /// </summary>
        public RealEstatePurchaseErrorCode ErrorCode { get; set; }
    }
}
