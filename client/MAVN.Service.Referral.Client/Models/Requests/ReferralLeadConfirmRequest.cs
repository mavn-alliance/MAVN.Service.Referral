using System;

namespace MAVN.Service.Referral.Client.Models.Requests
{
    /// <summary>
    /// Represents a model for creating a referral lead
    /// </summary>
    public class ReferralLeadConfirmRequest
    {
        /// <summary>
        /// The confirmation token of the referral.
        /// </summary>
        public string ConfirmationToken { get; set; }
    }
}
