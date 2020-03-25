using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.Referral.Client.Models.Requests;
using Lykke.Service.Referral.Client.Models.Responses;
using Refit;

namespace Lykke.Service.Referral.Client
{
    // This is an example of service controller interfaces.
    // Actual interface methods must be placed here (not in IReferralClient interface).

    /// <summary>
    /// Referral client API interface.
    /// </summary>
    [PublicAPI]
    public interface IReferralApi
    {
        /// <summary>
        /// Get referral code for a customer.
        /// </summary>
        /// <param name="customerId">The id of the customer</param>
        /// <returns>A referral model containing the referral code.</returns>
        [Get("/api/referrals/{customerId}")]
        Task<ReferralResultResponse> GetAsync(string customerId);

        /// <summary>
        /// Create referral code for a customer.
        /// </summary>
        /// <param name="referralCreate">The model containing customer for which referral would be created</param>
        /// <returns>A referral model containing the generated referral code.</returns>
        [Post("/api/referrals")]
        Task<ReferralCreateResponse> PostAsync(ReferralCreateRequest referralCreate);

        /// <summary>
        /// Return all friend referrals for customer.
        /// </summary>
        /// <param name="customerId">The Id of the customer.</param>
        /// <returns>A list of customer ids which the customer has referred for registration.</returns>
        [Get("/api/referrals/friends/{customerId}")]
        Task<ReferralCustomerHistoryResponse> GetCustomerFriendsReferralHistory(string customerId);
    }
}
