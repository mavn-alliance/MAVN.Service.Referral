using Lykke.Service.Referral.Client.Enums;

namespace Lykke.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Represents a result of a hotel referral usage attempt
    /// </summary>
    public class ReferralHotelUseResponse
    {
        /// <summary>
        /// Hotel referral
        /// </summary>
        public ReferralHotelModel HotelReferral { set; get; }
        
        /// <summary>
        /// Error code
        /// </summary>
        public ReferralHotelUseErrorCode ErrorCode { set; get; }
    }
}