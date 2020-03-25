using System;
using System.Collections.Generic;

namespace Lykke.Service.Referral.Client.Models.Responses.CommonReferral
{
    /// <summary>
    /// Represents a model containing referral by referral ids response
    /// </summary>
    public class CommonReferralByReferralIdsResponse
    {
        /// <summary>
        /// Represents a dictionary of common referrals with key Referral id
        /// </summary>
        public Dictionary<string, CommonReferralModel> CommonReferrals { get; set; }
    }
}
