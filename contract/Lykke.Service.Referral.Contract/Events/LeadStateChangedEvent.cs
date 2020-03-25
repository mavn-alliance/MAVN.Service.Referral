using System;
using Lykke.Service.Referral.Contract.Enums;

namespace Lykke.Service.Referral.Contract.Events
{
    /// <summary>
    /// Represents an Event, that will be published when lead's state is changed
    /// </summary>
    public class LeadStateChangedEvent
    {
        /// <summary>
        /// Represents the id of the changed lead
        /// </summary>
        public string LeadId { get; set; }

        /// <summary>
        /// Represents a timestamp of the change
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Represents the state of the lead
        /// </summary>
        public ReferralLeadState State { get; set; }
    }
}
