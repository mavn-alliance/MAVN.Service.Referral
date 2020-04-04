using MAVN.Service.Referral.Client.Enums;

namespace MAVN.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Represents a result of a hotel referral confirmation attempt
    /// </summary>
    public class ReferralHotelConfirmResponse
    {
        /// <summary>
        /// A hotel referral
        /// </summary>
        public ReferralHotelModel HotelReferral { set; get; }
        
        /// <summary>
        /// Error code
        /// </summary>
        public ReferralHotelConfirmErrorCode ErrorCode { set; get; }
    }
}