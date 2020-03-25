using Microsoft.WindowsAzure.Storage.Shared.Protocol;

namespace Lykke.Service.Referral.Client.Models.Requests
{
    /// <summary>
    /// Represents a request to confirm a hotel referral
    /// </summary>
    public class ReferralHotelConfirmRequest
    {
        /// <summary>
        /// Confirmation token
        /// </summary>
        public string ConfirmationToken { set; get; }
    }
}