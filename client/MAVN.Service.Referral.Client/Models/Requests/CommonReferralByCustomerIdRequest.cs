using System;
using System.Collections.Generic;
using MAVN.Service.Referral.Client.Enums;

namespace MAVN.Service.Referral.Client.Models.Requests
{
    /// <summary>
    /// Represents a model containing the data for Common referral by referrer id request
    /// </summary>
    public class CommonReferralByCustomerIdRequest
    {
        /// <summary>
        /// The id of the referrer
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The id of the campaign
        /// </summary>
        public Guid? CampaignId { get; set; }

        /// <summary>
        /// The statuses filter
        /// </summary>
        public IEnumerable<CommonReferralStatus> Statuses { get; set; }
    }
}   
