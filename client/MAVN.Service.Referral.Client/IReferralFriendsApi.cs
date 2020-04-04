using System.Threading.Tasks;
using MAVN.Service.Referral.Client.Models.Requests;
using MAVN.Service.Referral.Client.Models.Responses;
using Refit;

namespace MAVN.Service.Referral.Client
{
    /// <summary>
    /// Used to work with friend referrals
    /// </summary>
    public interface IReferralFriendsApi
    {
        /// <summary>
        /// Create friend referral
        /// </summary>
        /// <param name="referralFriendCreate">A model with creation details</param>
        /// <returns>A result of creation</returns>
        [Post("/api/referral-friends")]
        Task<ReferralFriendCreateResponse> CreateAsync(ReferralFriendCreateRequest referralFriendCreate);
    }
}
