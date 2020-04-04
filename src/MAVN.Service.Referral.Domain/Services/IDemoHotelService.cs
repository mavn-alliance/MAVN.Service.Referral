using System;
using System.Threading.Tasks;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.Domain.Services
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
