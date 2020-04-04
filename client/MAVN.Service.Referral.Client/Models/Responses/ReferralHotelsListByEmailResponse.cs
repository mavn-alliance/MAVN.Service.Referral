using System.Collections.Generic;

namespace MAVN.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Represents a result of a hotel referral get attempt
    /// </summary>
    public class ReferralHotelsListByEmailResponse
    {
        /// <summary>
        /// Hotel referrals
        /// </summary>
        public IReadOnlyList<ReferralHotelModel> HotelReferrals { get; set; }
    }
}