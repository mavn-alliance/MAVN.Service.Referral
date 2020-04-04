using System.Threading.Tasks;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.Domain.Services
{
    public interface IDemoLeadService
    {
        Task<ReferralLead> CreateReferralLeadAsync(ReferralLead referralLead);
        Task<ReferralLead> GetReferralLeadByConfirmationTokenAsync(string confirmationToken);
        Task ConfirmReferralLeadAsync(string confirmationToken);
    }
}
