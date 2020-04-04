using System.Collections.Generic;

namespace MAVN.Service.Referral.Client.Models.Responses.PropertyPurchase
{
    /// <summary>
    /// Represents a response from property purchase list
    /// </summary>
    public class PropertyPurchaseListResponse
    {
        /// <summary>
        /// List of property purchase model.
        /// </summary>
        public IReadOnlyList<PropertyPurchaseModel> PropertyPurchases { get; set; }
    }
}
