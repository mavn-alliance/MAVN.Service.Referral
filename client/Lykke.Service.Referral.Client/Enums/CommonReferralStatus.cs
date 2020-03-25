using System;

namespace Lykke.Service.Referral.Client.Enums
{
    /// <summary>
    /// Common referral status enumerator containing unified referral status
    /// </summary>
    public enum CommonReferralStatus
    {
        /// <summary>
        /// The referral is in pending
        /// </summary>
        Pending,

        /// <summary>
        /// The referral is confirmed and waiting acceptance
        /// </summary>
        Confirmed,

        /// <summary>
        /// The referral is accepted
        /// </summary>
        Accepted,

        /// <summary>
        /// The referral is expired or rejected
        /// </summary>
        Expired
    }
}
