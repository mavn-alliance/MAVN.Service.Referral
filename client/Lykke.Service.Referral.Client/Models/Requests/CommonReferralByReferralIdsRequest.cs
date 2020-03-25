using System;
using System.Collections.Generic;

namespace Lykke.Service.Referral.Client.Models.Requests
{
    /// <summary>
    /// Represents a model containing the data for Common referral by referral ids request
    /// </summary>
    public class CommonReferralByReferralIdsRequest
    {
        /// <summary>
        /// The list of referral ids
        /// </summary>
        public List<Guid> ReferralIds { get; set; }
    }
}   
