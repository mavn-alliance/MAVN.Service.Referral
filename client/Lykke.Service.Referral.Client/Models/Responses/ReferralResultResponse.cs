using Lykke.Service.Referral.Client.Enums;

namespace Lykke.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Represents Result from Referral Code request operation.
    /// </summary>
    public class ReferralResultResponse : ReferralErrorResponseModel
    {
        /// <summary>
        /// Represents referral code.
        /// </summary>
        public string ReferralCode { get; set; }
    }
}
