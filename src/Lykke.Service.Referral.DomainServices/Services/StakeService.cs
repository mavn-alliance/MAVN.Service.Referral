using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Campaign.Client;
using Lykke.Service.Campaign.Client.Models.Enums;
using Lykke.Service.Referral.Domain.Exceptions;
using Lykke.Service.Referral.Domain.Models;
using Lykke.Service.Referral.Domain.Services;
using Lykke.Service.Staking.Client;
using Lykke.Service.Staking.Client.Models;

namespace Lykke.Service.Referral.DomainServices.Services
{
    public class StakeService : IStakeService
    {
        private readonly ICampaignClient _campaignClient;
        private readonly IStakingClient _stakingClient;
        private readonly ILog _log;

        public StakeService(
            ICampaignClient campaignClient,
            IStakingClient stakingClient,
            ILogFactory logFactory)
        {
            _campaignClient = campaignClient;
            _stakingClient = stakingClient;
            _log = logFactory.CreateLog(this);
        }

        public async Task<ReferralStake> GetReferralStake(Guid? campaignId, string conditionType)
        {
            if (!campaignId.HasValue)
            {
                return null;
            }

            var campaign = await _campaignClient.Campaigns.GetByIdAsync(campaignId.Value.ToString("D"));

            if (campaign.ErrorCode == CampaignServiceErrorCodes.EntityNotFound)
            {
                throw new CampaignNotFoundException();
            }

            var condition = campaign.Conditions.FirstOrDefault(c => c.Type == conditionType);

            if (condition == null || !condition.HasStaking)
            {
                return null;
            }

            // Sanity check for the resharper
            if (condition.StakeAmount != null && condition.BurningRule != null &&
                condition.StakingPeriod != null && condition.StakeWarningPeriod != null)
            {
                return new ReferralStake
                {
                    Amount = condition.StakeAmount.Value,
                    BurnRatio = condition.BurningRule.Value,
                    CampaignId = campaignId.Value,
                    StakingPeriodInDays = condition.StakingPeriod.Value,
                    WarningPeriodInDays = condition.StakeWarningPeriod.Value
                };
            }
            else
            {
                // Log for the sanity check
                var e = new ArgumentException($"Staking for campaign '{campaignId}' is invalid.");
                _log.Error(e);
                throw e;
            }

        }

        public async Task SetStake(ReferralStake referralStake, string customerId, string referralId)
        {
            var response = await _stakingClient.ReferralStakesApi.ReferralStakeAsync(new ReferralStakeRequest
            {
                Amount = referralStake.Amount,
                BurnRatio = referralStake.BurnRatio,
                CampaignId = referralStake.CampaignId.ToString("D"),
                StakingPeriodInDays = referralStake.StakingPeriodInDays,
                WarningPeriodInDays = referralStake.WarningPeriodInDays,
                CustomerId = customerId,
                ReferralId = referralId
            });

            if (response.Error != ReferralStakeErrorCodes.None)
            {
                throw new InvalidReferralStakeException($"Staking for customer '{customerId}' and referral '{referralId}' failed with error code '{response.Error}'");
            }
        }

        public async Task ReleaseStake(string referralId, Guid campaignId, string conditionType)
        {
            var campaign = await _campaignClient.Campaigns.GetByIdAsync(campaignId.ToString("D"));

            if (campaign.ErrorCode == CampaignServiceErrorCodes.EntityNotFound)
            {
                throw new CampaignNotFoundException();
            }

            var condition = campaign.Conditions.FirstOrDefault(c => c.Type == conditionType);

            if (condition == null || !condition.HasStaking)
            {
                return;
            }

            // Sanity check for the resharper
            if (condition.StakingRule != null)
            {
                var stakedValue = 100 - condition.StakingRule.Value;

                var response = await _stakingClient.ReferralStakesApi.ReleaseReferralStakeAsync(new ReleaseReferralStakeRequest
                {
                    ReferralId = referralId,
                    BurnRatio = stakedValue
                });

                if (response.Error != ReferralStakeStatusUpdateErrorCodes.None)
                {
                    throw new InvalidReferralStakeReleaseException($"Release stake for referral '{referralId}' failed with error code '{response.Error}'");
                }
            }
            else
            {
                // Log for the sanity check
                var e = new ArgumentException($"Staking for campaign '{referralId}' is invalid.");
                _log.Error(e);
                throw e;
            }
        }
    }
}
