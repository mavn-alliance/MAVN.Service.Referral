using System;

namespace Lykke.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Used to represent count of Approved referrals by Agent's Id.
    /// </summary>
    public class ReferralLeadApprovedByAgentModel
    {
        /// <summary>
        /// Id of the Agent.
        /// </summary>
        public Guid AgentId { set; get; }
        
        /// <summary>
        /// Number of Approved referrals.
        /// </summary>
        public int Count { set; get; }
    }
}