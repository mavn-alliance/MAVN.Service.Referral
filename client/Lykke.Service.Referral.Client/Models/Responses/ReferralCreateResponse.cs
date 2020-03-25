namespace Lykke.Service.Referral.Client.Models.Responses
{
    /// <summary>
    /// Represents a response from creating customer referral code
    /// </summary>
    public class ReferralCreateResponse: ReferralErrorResponseModel
    {
        /// <summary>
        /// The referral code
        /// </summary>
        public string ReferralCode { get; set; }
    }
}
