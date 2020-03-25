using System.Collections.Generic;
using Lykke.Service.Referral.Client.Models.Responses.PropertyPurchase;

namespace Lykke.Service.Referral.Client.Models.Responses.CommonReferral
{
    /// <summary>
    /// A response model for referral bu customer id
    /// </summary>
    public class CommonReferralByCustomerIdResponse: ReferralErrorResponseModel
    {
        /// <summary>
        /// The list of common referrals
        /// </summary>
        public IReadOnlyCollection<CommonReferralModel> Referrals { get; set; }
    }
}
