using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.Domain.Repositories
{
    public interface IReferralHotelsRepository
    {
        Task<ReferralHotelEncrypted> CreateAsync(ReferralHotelEncrypted referralHotelEncrypted);

        Task<ReferralHotelEncrypted> UpdateAsync(ReferralHotelEncrypted referralHotelEncrypted);

        Task<IReadOnlyList<ReferralHotelEncrypted>> GetByEmailHashAsync(string emailHash, string partnerId, string location);

        Task<IReadOnlyList<ReferralHotelEncrypted>> GetByReferrerIdAsync(string referrerId);

        Task<IReadOnlyList<ReferralHotelEncrypted>> GetByReferrerIdAsync(
            string referrerId, 
            Guid? campaignId, 
            IEnumerable<ReferralHotelState> states);

        Task<ReferralHotelEncrypted> GetByConfirmationTokenAsync(string customerId);

        Task<IReadOnlyList<ReferralHotelEncrypted>> GetByReferralIdsAsync(List<Guid> referralIds);
    }
}
