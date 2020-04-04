using MAVN.Service.Referral.Client.Enums;

namespace MAVN.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Represents a result of a hotel referral creation attempt
    /// </summary>
    public class ReferralHotelCreateResponse
    {
        /// <summary>
        /// A hotel referral
        /// </summary>
        public ReferralHotelModel HotelReferral { get; set; }
        
        /// <summary>
        /// Error code
        /// </summary>
        public ReferralHotelCreateErrorCode ErrorCode { set; get; }
    }
}