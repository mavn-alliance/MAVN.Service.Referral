using Lykke.Service.Referral.Client.Enums;

namespace Lykke.Service.Referral.Client.Models
{
    /// <summary>
    /// Represents ReferralErrorResponseModel model 
    /// </summary>
    public class ReferralErrorResponseModel
    {
        /// <summary>
        /// Represents error code from the operation.
        /// </summary>
        public ReferralErrorCodes ErrorCode { get; set; }

        /// <summary>
        /// Represents error message from the operation.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
