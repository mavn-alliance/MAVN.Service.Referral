using System;

namespace MAVN.Service.Referral.Client.Models.Requests
{
    /// <summary>
    /// Represents a model for creating a referral friend
    /// </summary>
    public class ReferralFriendCreateRequest
    {
        /// <summary>
        /// The first name of the lead.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The email of the lead.
        /// Needs to be not null neither empty, should be a valid email.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// The id of the customer doing the referral.
        /// Needs to be not null neither empty.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// The id of the campaign the referral is for.
        /// Needs to be not null neither empty GUID.
        /// </summary>
        public Guid CampaignId { get; set; }
    }
}
