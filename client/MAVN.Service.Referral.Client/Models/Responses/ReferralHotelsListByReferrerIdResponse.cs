using System.Collections.Generic;
using MAVN.Service.Referral.Client.Enums;

namespace MAVN.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Represents a result of a hotel referral get attempt
    /// </summary>
    public class ReferralHotelsListByReferrerIdResponse
    {
        /// <summary>
        /// Hotel referrals
        /// </summary>
        public IReadOnlyList<ReferralHotelModel> HotelReferrals { get; set; }
        
        /// <summary>
        /// Error code
        /// </summary>
        public ReferralHotelsGetErrorCode ErrorCode { set; get; }
    }
}