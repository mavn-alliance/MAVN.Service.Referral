using System;

namespace Lykke.Service.Referral.Domain.Models
{
    /// <summary>
    /// Represents an encrypted referral hotel that used to store.
    /// </summary>
    public class ReferralHotelEncrypted
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The email SHA256 hash.
        /// </summary>
        public string EmailHash { get; set; }

        /// <summary>
        /// The phone number SHA256 hash.
        /// </summary>
        public string PhoneNumberHash { get; set; }

        /// <summary>
        /// The country code of phone number.
        /// </summary>
        public int PhoneCountryCodeId { get; set; }

        /// <summary>
        /// The name SHA256 hash.
        /// </summary>
        public string FullNameHash { get; set; }

        /// <summary>
        /// The referrer identifier.
        /// </summary>
        public string ReferrerId { get; set; }

        /// <summary>
        /// The location.
        /// </summary>
        public string Location { set; get; }

        /// <summary>
        /// The partner identifier.
        /// </summary>
        public string PartnerId { set; get; }

        /// <summary>
        /// The confirmation token.
        /// </summary>
        public string ConfirmationToken { get; set; }

        /// <summary>
        /// The status.
        /// </summary>
        public ReferralHotelState State { get; set; }

        /// <summary>
        /// The creation date and time.
        /// </summary>
        public DateTime CreationDateTime { get; set; }

        /// <summary>
        /// The date and time of expiration. 
        /// </summary>
        public DateTime ExpirationDateTime { get; set; }

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
