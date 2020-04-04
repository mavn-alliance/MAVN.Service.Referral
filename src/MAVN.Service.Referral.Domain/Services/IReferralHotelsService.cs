using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.Domain.Services
{
    public interface IReferralHotelsService
    {
        Task<ReferralHotel> CreateAsync(string email, string referrerId, Guid? campaignId, int phoneCountryCodeId, string phoneNumber, string fullName);

        Task<ReferralHotel> ConfirmAsync(string confirmationToken);

        Task<ReferralHotel> UseAsync(ReferralHotelUseModel useModel);
        Task<IReadOnlyList<ReferralHotelWithProfile>> GetByReferrerIdAsync(
            string customerId,
            Guid? campaignId,
            IEnumerable<ReferralHotelState> states);

        Task<IReadOnlyList<ReferralHotel>> GetByEmailAsync(string email, string partnerId, string location);

        Task<IReadOnlyList<ReferralHotelWithProfile>> GetReferralHotelsByReferralIdsAsync(List<Guid> referralIds);
    }
}
