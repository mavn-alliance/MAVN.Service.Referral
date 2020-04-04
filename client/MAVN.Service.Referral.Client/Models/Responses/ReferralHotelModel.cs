using System;
using MAVN.Service.Referral.Client.Enums;

namespace MAVN.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Represents a hotel referral
    /// </summary>
    public class ReferralHotelModel
    {
        /// <summary>
        /// Unique Id of the referral
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Email of the person referred
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Id of the Customer doing the referral
        /// </summary>
        public string ReferrerId { get; set; }
        
        /// <summary>
        /// Confirmation token
        /// </summary>
        public string ConfirmationToken { get; set; }
        
        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// Id of the Partner
        /// </summary>
        public string PartnerId { get; set; }
        
        /// <summary>
        /// State of the referral
        /// </summary>
        public ReferralHotelState State { get; set; }

        /// <summary>
        /// Date and time of the referral creation
        /// </summary>
        public DateTime CreationDateTime { get; set; }

        /// <summary>
        /// Date and time when referral becomes expired
        /// </summary>
        public DateTime ExpirationDateTime { get; set; }

        /// <summary>
        /// ID of the customer in customer profile if the lead is customer
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// ID of the campaign if any
        /// </summary>
        public Guid? CampaignId { get; set; }

        /// <summary>
        /// Phone number of the lead
        /// </summary>
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// Country code of the phone number
        /// </summary>
        public int PhoneCountryCodeId { set; get; }

        /// <summary>
        /// The first name of the referral lead
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the referral lead
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The name of the referral lead
        /// </summary>
        public string FullName { get; set; }
    }
}
