using System;
using System.Threading.Tasks;
using Lykke.Service.Referral.Domain.Models;

namespace Lykke.Service.Referral.Domain.Services
{
    public interface IStakeService
    {
        Task<ReferralStake> GetReferralStake(Guid? campaignId, string conditionType);

        Task SetStake(ReferralStake referralStake, string customerId, string referralId);

        Task ReleaseStake(string referralId, Guid campaignId, string conditionType);
    }
}
