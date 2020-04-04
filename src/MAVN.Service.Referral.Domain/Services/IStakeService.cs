using System;
using System.Threading.Tasks;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.Domain.Services
{
    public interface IStakeService
    {
        Task<ReferralStake> GetReferralStake(Guid? campaignId, string conditionType);

        Task SetStake(ReferralStake referralStake, string customerId, string referralId);

        Task ReleaseStake(string referralId, Guid campaignId, string conditionType);
    }
}
