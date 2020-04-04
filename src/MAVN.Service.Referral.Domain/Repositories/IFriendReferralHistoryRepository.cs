using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.Domain.Repositories
{
    public interface IFriendReferralHistoryRepository
    {
        Task<FriendReferralHistory> GetAllReferralsForCustomerAsync(Guid customerId);
        Task<ReferralFriend> CreateAsync(ReferralFriend referral);
        Task UpdateAsync(ReferralFriend referral);
        Task<ReferralFriend> GetByIdAsync(Guid referralFriendId);
        Task<ReferralFriend> GetAcceptedAsync(Guid customerId);
        Task<IReadOnlyList<ReferralFriend>> GetByReferrerIdAsync(
            Guid referrerId,
            Guid? campaignId, 
            IEnumerable<ReferralFriendState> states);
        Task<IReadOnlyList<ReferralFriend>> GetReferralFriendsByReferralIdsAsync(List<Guid> referralIds);
    }
}
