using System;
using Lykke.Service.Referral.Domain.Entities;

namespace Lykke.Service.Referral.Domain.Models
{
    /// <summary>
    /// Represents an encrypted referral lead that used to store.
    /// </summary>
    public class ReferralLeadEncrypted
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The phone country code identifier.
        /// </summary>
        public int PhoneCountryCodeId { get; set; }

        /// <summary>
        /// The SHA256 hash of phone number.
        /// </summary>
        public string PhoneNumberHash { get; set; }

        /// <summary>
        /// The SHA256 hash of email address.
        /// </summary>
        public string EmailHash { get; set; }

        /// <summary>
        /// The agent identifier.
        /// </summary>
        public Guid AgentId { get; set; }

        /// <summary>
        /// The salesforce agent identifier.
        /// </summary>
        public string AgentSalesforceId { get; set; }

        /// <summary>
        /// The referral lead salesforce identifier. 
        /// </summary>
        public string SalesforceId { get; set; }

        /// <summary>
        /// The salesforce registration status.
        /// </summary>
        public string ResponseStatus { get; set; }
        
        /// <summary>
        /// The token used to confirm the referral.
        /// </summary>
        public string ConfirmationToken { get; set; }

        /// <summary>
        /// The state of the referral lead.
        /// </summary>
        public ReferralLeadState State { get; set; }
        
        /// <summary>
        /// The date and time of creation.
        /// </summary>
        public DateTime CreationDateTime { get; set; }

        /// <summary>
        /// The campaign id if any.
        /// </summary>
        public Guid? CampaignId { get; set; }

        /// <summary>
        /// Is stake enabled for Campaign.
        /// </summary>
        public bool StakeEnabled { get; set; }
    }
}
