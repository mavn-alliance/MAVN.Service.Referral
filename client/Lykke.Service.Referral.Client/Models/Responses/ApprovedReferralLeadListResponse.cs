using System.Collections.Generic;

namespace Lykke.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Represents a response from approved referral lead list
    /// </summary>
    public class ApprovedReferralLeadListResponse : ReferralErrorResponseModel
    {
        /// <summary>
        /// List of referral lead model.
        /// </summary>
        public IReadOnlyList<ApprovedReferralLeadModel> ReferralLeads { get; set; }
    }
}
