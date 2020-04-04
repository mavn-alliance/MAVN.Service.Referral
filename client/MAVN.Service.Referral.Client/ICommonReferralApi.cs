using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.Referral.Client.Models.Requests;
using MAVN.Service.Referral.Client.Models.Responses.CommonReferral;
using Refit;

namespace MAVN.Service.Referral.Client
{
    /// <summary>
    /// Used to work with referral in a common way
    /// </summary>
    [PublicAPI]
    public interface ICommonReferralApi
    {
        /// <summary>
        /// Get Common referral by customer who made them.
        /// </summary>
        /// <param name="request">The request model.</param>
        /// <returns></returns>
        [Post("/api/common-referral/byCustomer")]
        Task<CommonReferralByCustomerIdResponse> GetReferralsByCustomerIdAsync(CommonReferralByCustomerIdRequest request);
        
        /// <summary>
        /// Get Common referral by list of referral ids.
        /// </summary>
        /// <param name="request">The request model.</param>
        /// <returns></returns>
        [Post("/api/common-referral/list")]
        Task<CommonReferralByReferralIdsResponse> GetReferralsByReferralIdsAsync(CommonReferralByReferralIdsRequest request);
    }
}
