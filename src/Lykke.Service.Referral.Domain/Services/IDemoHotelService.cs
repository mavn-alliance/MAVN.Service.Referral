using System;
using System.Threading.Tasks;
using Lykke.Service.Referral.Domain.Models;

namespace Lykke.Service.Referral.Domain.Services
{
    public interface IDemoHotelService
    {
        Task<ReferralHotel> CreateHotelReferralAsync(
            Guid? campaignId,
            string email,
            string referrerId,
            string fullName,
            int phoneCountryCodeId,
            string phoneNumber);
        Task<ReferralHotel> GetReferralHotelByConfirmationTokenAsync(string confirmationToken);
        Task<ReferralHotel> ConfirmReferralHotelAsync(string confirmationToken);
    }
}
