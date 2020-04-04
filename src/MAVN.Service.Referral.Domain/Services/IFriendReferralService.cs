using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.Domain.Services
{
    public interface IFriendReferralService
    {
        Task<FriendReferralHistory> GetAllReferralsForCustomerAsync(Guid customerId);
        Task<ReferralFriend> CreateAsync(Guid referrerId, Guid campaignId, string email, string fullName);
        Task<(bool isSuccessful, string errorMessage)> ConfirmAsync(string referralCode, Guid referredId);
        Task<(bool isSuccessful, string errorMessage)> AcceptAsync(Guid customerId);
        Task<IReadOnlyList<ReferralFriend>> GetByReferrerIdAsync(
            Guid referrerId, 
            Guid? campaignId,
            IEnumerable<ReferralFriendState> states);

        Task<IReadOnlyList<ReferralFriend>> GetReferralFriendsByReferralIdsAsync(List<Guid> referralIds);
    }
}
