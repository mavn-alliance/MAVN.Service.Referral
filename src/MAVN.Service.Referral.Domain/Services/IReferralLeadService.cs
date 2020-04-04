using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.Referral.Domain.Entities;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.Domain.Services
{
    public interface IReferralLeadService
    {
        Task<ReferralLead> CreateReferralLeadAsync(ReferralLead referralLead);
        Task<IReadOnlyList<ReferralLeadWithDetails>> GetReferralLeadsForReferrerAsync(
            Guid referrerId,
            Guid? campaignId,
            IEnumerable<ReferralLeadState> states);
        Task<IReadOnlyList<ReferralLead>> GetApprovedLeadsAsync();
        Task<ReferralLead> ConfirmReferralLeadAsync(string confirmationToken);
        Task<ReferralLead> ApproveReferralLeadAsync(Guid referralId, DateTime timestamp);
        Task<ReferralLead> RejectReferralLeadAsync(Guid referralId, DateTime timestamp);
        Task<LeadStatisticModel> GetStatistic();
        Task<Dictionary<Guid, int>> GetApprovedReferralsCountByAgentsAsync(List<Guid> agentIds);
        Task<IReadOnlyList<ReferralLeadWithDetails>> GetReferralLeadsByReferralIdsAsync(List<Guid> referralIds);
    }
}
