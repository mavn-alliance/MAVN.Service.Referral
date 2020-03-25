using System;

namespace Lykke.Service.Referral.Client.Models.Requests
{
    /// <summary>
    /// Represents a model for creating a referral code for customer
    /// </summary>
    public class ReferralCreateRequest
    {
        /// <summary>
        /// The customer's id.
        /// Needs to be not null neither empty.
        /// </summary>
        public string CustomerId { get; set; }
    }
}
