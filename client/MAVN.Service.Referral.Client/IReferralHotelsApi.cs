using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.Referral.Client.Models.Requests;
using MAVN.Service.Referral.Client.Models.Responses;
using Refit;

namespace MAVN.Service.Referral.Client
{
    /// <summary>
    /// Used to work with hotel referrals
    /// </summary>
    [PublicAPI]
    public interface IReferralHotelsApi
    {
        /// <summary>
        /// Create hotel referral
        /// </summary>
        /// <param name="referralHotelCreate">A model with creation details</param>
        /// <returns>A result of creation</returns>
        [Post("/api/referral-hotels")]
        Task<ReferralHotelCreateResponse> CreateAsync(ReferralHotelCreateRequest referralHotelCreate);

        /// <summary>
        /// Confirm hotel referral
        /// </summary>
        /// <param name="referralHotelConfirm">A model with confirmation details</param>
        /// <returns>A result of confirmation</returns>
        [Put("/api/referral-hotels/confirm")]
        Task<ReferralHotelConfirmResponse> ConfirmAsync(ReferralHotelConfirmRequest referralHotelConfirm);

        /// <summary>
        /// Use hotel referral
        /// </summary>
        /// <param name="referralHotelUse">A model with usage details</param>
        /// <returns>A result of usage</returns>
        [Put("/api/referral-hotels/use")]
        Task<ReferralHotelUseResponse> UseAsync(ReferralHotelUseRequest referralHotelUse);

        /// <summary>
        /// Get hotel referrals by Referrer Id
        /// </summary>
        /// <param name="referrerId">Id of the Customer</param>
        /// <returns>A list of hotel referrals</returns>
        [Get("/api/referral-hotels/byReferrerId")]
        Task<ReferralHotelsListByReferrerIdResponse> GetByReferrerIdAsync(string referrerId);

        /// <summary>
        /// Get hotel referrals by Email
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <returns>A list of hotel referrals</returns>
        [Post("/api/referral-hotels/getbyemail")]
        Task<ReferralHotelsListByEmailResponse> GetByEmailAsync(GetHotelReferralsByEmailRequestModel request);
    }
}
