using System.Collections;
using System.Collections.Generic;
using Lykke.Service.Referral.Client.Enums;

namespace Lykke.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Represents response of a referral history for customer
    /// </summary>
    public class PurchasesReferralHistoryResponse : ReferralErrorResponseModel
    {
        /// <summary>
        /// List of referred customers
        /// </summary>
        public IEnumerable<string> ReferredCustomers { get; set; }
    }
}
