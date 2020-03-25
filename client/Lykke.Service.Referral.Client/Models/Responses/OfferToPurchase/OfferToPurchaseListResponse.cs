using System.Collections.Generic;

namespace Lykke.Service.Referral.Client.Models.Responses.OfferToPurchase
{
    /// <summary>
    /// Represents a response from offers to purchase list request
    /// </summary>
    public class OfferToPurchaseListResponse
    {
        /// <summary>
        /// List of offer to purchase model.
        /// </summary>
        public IReadOnlyList<OfferToPurchaseModel> OfferToPurchases { get; set; }
    }
}
