using System.Collections;
using System.Collections.Generic;
using MAVN.Service.Referral.Client.Enums;

namespace MAVN.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Represents response of a referral history for customer
    /// </summary>
    public class ReferralCustomerHistoryResponse: ReferralErrorResponseModel
    {
        /// <summary>
        /// List of referred customers
        /// </summary>
        public IEnumerable<string> ReferredCustomers { get; set; }
    }
}
