using System;

namespace MAVN.Service.Referral.Client.Models.Requests
{
    /// <summary>
    /// Represents a model for creating a referral lead
    /// </summary>
    public class ReferralLeadCreateRequest
    {
        /// <summary>
        /// The first name of the lead.
        /// Needs to be not null neither empty with length between 3 and 100.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the lead.
        /// Needs to be not null neither empty with length between 3 and 100.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The phone number country code id of the lead.
        /// Needs to be not null neither empty.
        /// </summary>
        public int PhoneCountryCodeId { get; set; }

        /// <summary>
        /// The phone number of the lead.
        /// Needs to be not null neither empty with length between 2 and 50.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The email of the lead.
        /// Needs to be not null neither empty, should be a valid email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The lead note.
        /// Can be either null or between 2 or 1000 in length.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// The id of the customer doing the referral.
        /// Needs to be not null neither empty.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// The campaign id if any.
        /// </summary>
        public Guid? CampaignId { get; set; }
    }
}
