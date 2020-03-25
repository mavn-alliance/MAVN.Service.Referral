using System;
using Lykke.Service.Referral.Client.Enums;

namespace Lykke.Service.Referral.Client.Models.Responses.CommonReferral
{
    /// <summary>
    /// Model containing information for common referral
    /// </summary>
    public class CommonReferralModel
    {
        /// <summary>
        /// Unique Id of the referral
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The first name of the referred customer
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the referred customer.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Email of the referred customer
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// TimeStamp of the referral
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The status of the referral
        /// </summary>
        public CommonReferralStatus Status { get; set; }

        /// <summary>
        /// The type of the referral
        /// </summary>
        public ReferralType ReferralType { get; set; }

        /// <summary>
        /// ID of the campaign if any
        /// </summary>
        public Guid? CampaignId { get; set; }

        /// <summary>
        /// ID of the partner if any
        /// </summary>
        public Guid? PartnerId { get; set; }
    }
}
