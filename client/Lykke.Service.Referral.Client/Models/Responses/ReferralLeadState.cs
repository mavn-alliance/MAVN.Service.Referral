using JetBrains.Annotations;

namespace Lykke.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Specifies status of referral lead. 
    /// </summary>
    [PublicAPI]
    public enum ReferralLeadState
    {
        /// <summary>
        /// Indicates that the referral lead waiting for approval. 
        /// </summary>
        Pending,
        
        /// <summary>
        /// Indicates that the referral lead confirmed.
        /// </summary>
        Confirmed,
        
        /// <summary>
        /// Indicates that the referral lead approved.
        /// </summary>
        Approved,
        
        /// <summary>
        /// Indicates that the referral lead rejected.
        /// </summary>
        Rejected
    }
}
