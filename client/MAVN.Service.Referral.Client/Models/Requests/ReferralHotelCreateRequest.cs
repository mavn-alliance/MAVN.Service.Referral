using System;

namespace MAVN.Service.Referral.Client.Models.Requests
{
    /// <summary>
    /// Represents a request to create a hotel referral
    /// </summary>
    public class ReferralHotelCreateRequest
    {
        /// <summary>
        /// The email of the lead.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The Id of the Customer doing the referral.
        /// </summary>
        public string ReferrerId { get; set; }

        /// <summary>
        /// The Id of the Campaign if any.
        /// </summary>
        public Guid? CampaignId { get; set; }

        /// <summary>
        /// The phone number of referred person.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Country code of the phone number.
        /// </summary>
        public int PhoneCountryCodeId { get; set; }

        /// <summary>
        /// The name of referred person.
        /// </summary>
        public string FullName { get; set; }
    }
}
