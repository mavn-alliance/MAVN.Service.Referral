using System.Collections.Generic;

namespace Lykke.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Represents a response from referral lead list
    /// </summary>
    public class ReferralLeadListResponse : ReferralErrorResponseModel
    {
        /// <summary>
        /// List of referral lead model.
        /// </summary>
        public IReadOnlyList<ReferralLeadModel> ReferralLeads { get; set; }
    }
}
