using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.Referral.Domain.Exceptions;
using MAVN.Service.Referral.Domain.Managers;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.Domain.Services;

namespace MAVN.Service.Referral.DomainServices.Services
{
    public class ReferralService: IReferralService
    {
        private readonly IReferralRepository _referralRepository;
        private readonly int _referralCodeSize;
        private readonly IHashingManager _hashingManager;
        private readonly ILog _log;

        public ReferralService(
            IReferralRepository referralRepository, 
            int referralCodeSize,
            IHashingManager hashingManager,
            ILogFactory logFactory)
        {
            _referralRepository = referralRepository;
            _referralCodeSize = referralCodeSize;
            _hashingManager = hashingManager;
            _log = logFactory.CreateLog(this);
        }

        public async Task<Domain.Models.Referral> GetReferralByCustomerIdAsync(Guid customerId)
        {
            return await _referralRepository.GetByCustomerIdAsync(customerId) ?? 
                   throw new CustomerNotFoundException($"Referral code for Customer with id '{customerId}' not found.");
        }

        public async Task<Domain.Models.Referral> GetReferralByReferralCodeAsync(string referralCode)
        {
            if (string.IsNullOrEmpty(referralCode))
            {
                var exception = new ArgumentNullException(nameof(referralCode));
                _log.Error("Input parameter is null.", exception);
                throw exception;
            }

            return await _referralRepository.GetByReferralCodeAsync(referralCode) ??
                   throw new CustomerNotFoundException($"Referral code for Customer with id '{referralCode}' not found.");
        }

        public async Task<string> GetOrCreateReferralForCustomerIdAsync(Guid customerId)
        {
            var existingReferral = await _referralRepository.GetByCustomerIdAsync(customerId);
            if (existingReferral != null)
            {
                return existingReferral.ReferralCode;
            }

            var referralCode = await GenerateReferralCodeAsync(customerId.ToString("D"));

            await _referralRepository.CreateIfNotExistAsync(new Domain.Models.Referral()
            {
                CustomerId = customerId,
                Id = Guid.NewGuid().ToString("D"),
                ReferralCode = referralCode
            });

            return referralCode;
        }

        public async Task CreateReferralForCustomerIfNotExistAsync(Guid customerId)
        {
            var existingCustomerReferral = await _referralRepository.GetByCustomerIdAsync(customerId);

            if (existingCustomerReferral == null)
            {
                await GetOrCreateReferralForCustomerIdAsync(customerId);
            }
        }

        private async Task<string> GenerateReferralCodeAsync(string customerId)
        {
            var hash = _hashingManager.GenerateBase(customerId);
            var referralCode = hash.Substring(0, _referralCodeSize);

            var offset = 0;
            while (await _referralRepository.GetByReferralCodeAsync(referralCode) != null)
            {
                _log.Warning($"Collision for {referralCode} found.");
                offset++;
                if (offset + _referralCodeSize > hash.Length)
                {
                    offset = 0;
                    hash = _hashingManager.GenerateBase(hash);
                }

                referralCode = hash.Substring(offset, _referralCodeSize);
            }

            return referralCode;
        }
    }
}
