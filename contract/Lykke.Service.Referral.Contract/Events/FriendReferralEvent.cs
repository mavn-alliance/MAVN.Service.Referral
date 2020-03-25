using System;

namespace Lykke.Service.Referral.Contract.Events
{
    /// <summary>
    /// Represents Friend referral event, that will be published when new customer is referred
    /// </summary>
    public class FriendReferralEvent
    {
        /// <summary>
        /// Represents the Id of the referrer
        /// </summary>
        public string ReferrerId { get; set; }

        /// <summary>
        /// Represents the id of the referred customer
        /// </summary>
        public string ReferredId { get; set; }

        /// <summary>
        /// Represents a timestamp of the referral
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Represents a referral's id
        /// </summary>
        public string ReferralId { get; set; }
    }
}
