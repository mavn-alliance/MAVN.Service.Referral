using System.Threading.Tasks;
using Lykke.Service.Referral.Domain.Models;

namespace Lykke.Service.Referral.Domain.Services
{
    public interface IDemoLeadService
    {
        Task<ReferralLead> CreateReferralLeadAsync(ReferralLead referralLead);
        Task<ReferralLead> GetReferralLeadByConfirmationTokenAsync(string confirmationToken);
        Task ConfirmReferralLeadAsync(string confirmationToken);
    }
}
