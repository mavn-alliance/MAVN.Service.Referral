using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Referral.Domain.Entities;
using Lykke.Service.Referral.Domain.Models;

namespace Lykke.Service.Referral.Domain.Repositories
{
    public interface IReferralLeadRepository
    {
        Task<ReferralLeadEncrypted> CreateAsync(ReferralLeadEncrypted referralLeadEncrypted);

        Task<ReferralLeadEncrypted> UpdateAsync(ReferralLeadEncrypted referralLeadEncrypted);

        Task<IReadOnlyList<ReferralLeadEncrypted>> GetByEmailHashAsync(string emailHash);

        Task<IReadOnlyList<ReferralLeadEncrypted>> GetByPhoneNumberHashAsync(int countryCodeId, string phoneNumberHash);

        Task<ReferralLeadEncrypted> GetByConfirmationTokenAsync(string confirmationToken);

        Task<IReadOnlyList<ReferralLeadEncryptedWithDetails>> GetForReferrerAsync(
            Guid referrerId, 
            Guid? campaignId, 
            IEnumerable<ReferralLeadState> states);

        Task<IReadOnlyList<ReferralLeadEncrypted>> GetApprovedAsync();

        Task<ReferralLeadEncrypted> GetAsync(Guid referLeadId);

        Task<bool> DoesExistAsync(Guid referLeadId);

        Task<int> GetCountAsync(ReferralLeadState? status = null);

        Task<Dictionary<Guid, int>> GetApprovedReferralsCountByAgentsAsync(List<Guid> agentIds);

        Task<IReadOnlyList<ReferralLeadEncryptedWithDetails>> GetByReferralIdsAsync(List<Guid> referralIds);
    }
}
