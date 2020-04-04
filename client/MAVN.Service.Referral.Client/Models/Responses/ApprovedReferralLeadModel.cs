using System;

namespace MAVN.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Model representing approved lead model
    /// </summary>
    public class ApprovedReferralLeadModel
    {
        /// <summary>
        /// The id of the approved lead
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The referral id of the approved lead
        /// </summary>
        public string ReferralLeadId { get; set; }

        /// <summary>
        /// The salesforce id of the approved lead
        /// </summary>
        public string SalesforceId { get; set; }

        /// <summary>
        /// The timestamp of the approved lead
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
