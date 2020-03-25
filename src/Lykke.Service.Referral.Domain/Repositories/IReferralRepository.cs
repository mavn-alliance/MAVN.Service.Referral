using System;
using System.Threading.Tasks;

namespace Lykke.Service.Referral.Domain.Repositories
{
    public interface IReferralRepository
    {
        Task<Models.Referral> GetByCustomerIdAsync(Guid customerId);

        Task<Models.Referral> GetByReferralCodeAsync(string referralCode);

        Task<bool> CreateIfNotExistAsync(Domain.Models.Referral referral);
    }
}
