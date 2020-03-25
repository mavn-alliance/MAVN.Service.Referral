using System;

namespace Lykke.Service.Referral.Contract.Events
{
    /// <summary>
    /// Represents an event sent when property lead is approved
    /// </summary>
    public class PropertyLeadApprovedReferralEvent
    {
        /// <summary>
        /// Represents the Id of the referrer
        /// </summary>
        public string ReferrerId { get; set; }

        /// <summary>
        /// Represents a timestamp of the referral
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Represents a Campaign id if any
        /// </summary>
        public Guid? StakedCampaignId { get; set; }

        /// <summary>
        /// Represents a referral's id
        /// </summary>
        public string ReferralId { get; set; }
    }
}
