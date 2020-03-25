using System;
using System.Threading.Tasks;

namespace Lykke.Service.Referral.Domain.Services
{
    public interface IReferralService
    {
        Task<Models.Referral> GetReferralByCustomerIdAsync(Guid customerId);
        Task<Models.Referral> GetReferralByReferralCodeAsync(string referralCode);
        Task<string> GetOrCreateReferralForCustomerIdAsync(Guid customerId);
        Task CreateReferralForCustomerIfNotExistAsync(Guid customerId);
    }
}
