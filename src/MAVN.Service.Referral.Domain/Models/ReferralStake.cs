using System;
using MAVN.Numerics;

namespace MAVN.Service.Referral.Domain.Models
{
    public class ReferralStake
    {
        public Money18 Amount { get; set; }
        public decimal BurnRatio { get; set; }
        public Guid CampaignId { get; set; }
        public int StakingPeriodInDays { get; set; }
        public int WarningPeriodInDays { get; set; }
    }
}
