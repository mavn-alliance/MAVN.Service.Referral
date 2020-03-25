using System;

namespace Lykke.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Model representing referral lead model
    /// </summary>
    public class ReferralLeadModel
    {
        /// <summary>
        /// The id of the referral lead
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The first name of the referral lead
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the referral lead
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The country code id of the referral lead
        /// </summary>
        public int PhoneCountryCodeId { get; set; }

        /// <summary>
        /// The phone number of the referral lead
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The email of the referral lead
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The note for the referral lead
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// The agent doing the referral
        /// </summary>
        public string AgentId { get; set; }

        /// <summary>
        /// The salesforce id of the agent doing the referral
        /// </summary>
        public string AgentSalesforceId { get; set; }

        /// <summary>
        /// The state of the referral lead
        /// </summary>
        public ReferralLeadState State { get; set; }

        /// <summary>
        /// The salesforce id of the referral lead
        /// </summary>
        public string SalesforceId { get; set; }

        /// <summary>
        /// Token used to confirm the referral
        /// </summary>
        public string ConfirmationToken { get; set; }

        /// <summary>
        /// The creation date and time of the referral lead
        /// </summary>
        public DateTime CreationDateTime { get; set; }
        
        /// <summary>
        /// The number of SPA (sales purchase agreement)
        /// </summary>
        public int PurchaseCount { get; set; }

        /// <summary>
        /// The number of OTP (offers to purchase).
        /// </summary>
        public int OffersCount { get; set; }

        /// <summary>
        /// The Campaign Id if any.
        /// </summary>
        public Guid? CampaignId { get; set; }
    }
}
