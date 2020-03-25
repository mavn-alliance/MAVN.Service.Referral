using System;
using Lykke.Service.Referral.Domain.Models;
using Lykke.Service.Referral.Domain.Repositories;
using Lykke.Service.Referral.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Referral.Domain.Exceptions;
using Lykke.Service.Referral.DomainServices.Managers;

namespace Lykke.Service.Referral.DomainServices.Services
{
    public class PropertyPurchaseService : IPropertyPurchaseService
    {
        private const string EstatePurchaseConditionName = "estate-lead-referral";

        private readonly IReferralLeadRepository _referralLeadRepository;
        private readonly IPropertyPurchaseRepository _propertyPurchaseRepository;
        private readonly IStakeService _stakeService;
        private readonly CommissionManager _commissionManager;

        public PropertyPurchaseService(
            IReferralLeadRepository referralLeadRepository,
            IPropertyPurchaseRepository propertyPurchaseRepository,
            IStakeService stakeService,
            CommissionManager commissionManager)
        {
            _referralLeadRepository = referralLeadRepository;
            _propertyPurchaseRepository = propertyPurchaseRepository;
            _stakeService = stakeService;
            _commissionManager = commissionManager;
        }

        public async Task<IReadOnlyList<PropertyPurchase>> GetPropertyPurchasesAsync()
        {
            return await _propertyPurchaseRepository.GetAsync();
        }

        public async Task<Guid> AddRealEstatePurchase(PropertyPurchase propertyPurchase)
        {
            var lead = await _referralLeadRepository.GetAsync(propertyPurchase.ReferralLeadId);
            if (lead == null)
            {
                throw new ReferralDoesNotExistException($"Referral lead with id '{propertyPurchase.ReferralLeadId} does not exist.'");
            }

            // If duplicate comes we want to skip releasing stake and inserting new entity
            if (!await _propertyPurchaseRepository.PropertyPurchaseExistsAsync(propertyPurchase.ReferralLeadId))
            {
                await _propertyPurchaseRepository.InsertAsync(propertyPurchase);

                if (lead.StakeEnabled)
                {
                    await _stakeService.ReleaseStake(lead.Id.ToString("D"), lead.CampaignId.Value, EstatePurchaseConditionName);
                }
            }

            return lead.CampaignId.Value;
        }
    }
}
