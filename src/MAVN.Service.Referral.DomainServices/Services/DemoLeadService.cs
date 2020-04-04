using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Falcon.Common;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.AgentManagement.Client;
using Lykke.Service.AgentManagement.Client.Models.Agents;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.CustomerProfile.Client.Models.Enums;
using Lykke.Service.CustomerProfile.Client.Models.Requests;
using Lykke.Service.Dictionaries.Client;
using MAVN.Service.Referral.Contract.Events;
using MAVN.Service.Referral.Domain.Entities;
using MAVN.Service.Referral.Domain.Exceptions;
using MAVN.Service.Referral.Domain.Managers;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.Domain.Services;
using MAVN.Service.Referral.DomainServices.Extensions;
using MAVN.Service.Referral.DomainServices.Managers;

namespace MAVN.Service.Referral.DomainServices.Services
{
    public class DemoLeadService: IDemoLeadService
    {
        private readonly IReferralLeadRepository _referralLeadRepository;
        private readonly ISettingsService _settingsService;
        private readonly INotificationPublisherService _notificationPublisherService;
        private readonly IHashingManager _hashingManager;
        private readonly IAgentManagementClient _agentManagementClient;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IDictionariesClient _dictionariesClient;
        private readonly CommissionManager _commissionManager;
        private readonly int _delay;
        private readonly IOfferToPurchasePurchaseRepository _offerToPurchasePurchaseRepository;
        private readonly IRabbitPublisher<PropertyLeadApprovedReferralEvent> _propertyLeadApprovedReferralPublisher;
        private readonly IRabbitPublisher<OfferToPurchaseByLeadEvent> _offerToPurchasePublisher;
        private readonly IRabbitPublisher<LeadStateChangedEvent> _leadStateChangedPublisher;
        private readonly IPropertyPurchaseRepository _propertyPurchaseRepository;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public DemoLeadService(
            IReferralLeadRepository referralLeadRepository,
            ISettingsService settingsService,
            INotificationPublisherService notificationPublisherService,
            IHashingManager hashingManager,
            IAgentManagementClient agentManagementClient,
            ICustomerProfileClient customerProfileClient,
            IDictionariesClient dictionariesClient,
            CommissionManager commissionManager,
            ILogFactory logFactory,
            int delay,
            IOfferToPurchasePurchaseRepository offerToPurchasePurchaseRepository,
            IRabbitPublisher<PropertyLeadApprovedReferralEvent> propertyLeadApprovedReferralPublisher,
            IRabbitPublisher<OfferToPurchaseByLeadEvent> offerToPurchasePublisher,
            IRabbitPublisher<LeadStateChangedEvent> leadStateChangedPublisher,
            IMapper mapper,
            IPropertyPurchaseRepository propertyPurchaseRepository)
        {
            _referralLeadRepository = referralLeadRepository;
            _settingsService = settingsService;
            _notificationPublisherService = notificationPublisherService;
            _hashingManager = hashingManager;
            _agentManagementClient = agentManagementClient;
            _customerProfileClient = customerProfileClient;
            _dictionariesClient = dictionariesClient;
            _commissionManager = commissionManager;
            _delay = delay;
            _offerToPurchasePurchaseRepository = offerToPurchasePurchaseRepository;
            _propertyLeadApprovedReferralPublisher = propertyLeadApprovedReferralPublisher;
            _offerToPurchasePublisher = offerToPurchasePublisher;
            _leadStateChangedPublisher = leadStateChangedPublisher;
            _propertyPurchaseRepository = propertyPurchaseRepository;
            _mapper = mapper;
            _log = logFactory.CreateLog(this);
        }

        public async Task<ReferralLead> CreateReferralLeadAsync(ReferralLead referralLead)
        {
            ReferralLead createdReferralLead = null;

            try
            {
                var agentCustomer = await _customerProfileClient.CustomerProfiles
                    .GetByCustomerIdAsync(referralLead.AgentId.ToString());

                if (agentCustomer.ErrorCode == CustomerProfileErrorCodes.CustomerProfileDoesNotExist)
                {
                    throw new CustomerDoesNotExistException($"Customer '{referralLead.AgentId}' does not exist.");
                }

                var agent = await _agentManagementClient.Agents.GetByCustomerIdAsync(referralLead.AgentId);

                if (agent == null || agent.Status != AgentStatus.ApprovedAgent)
                {
                    throw new CustomerNotApprovedAgentException("Customer isn't an approved agent.");
                }

                var countryPhoneCode = await _dictionariesClient.Salesforce
                   .GetCountryPhoneCodeByIdAsync(referralLead.PhoneCountryCodeId);

                if (countryPhoneCode == null)
                {
                    throw new CountryCodeDoesNotExistException(
                        $"Country information for Country code id '{referralLead.PhoneCountryCodeId}' does not exist.");
                }

                var emailHash = referralLead.Email.ToSha256Hash();

                var phoneNumberE164 = PhoneUtils.GetE164FormattedNumber(referralLead.PhoneNumber, countryPhoneCode.IsoCode);

                if (string.IsNullOrEmpty(phoneNumberE164))
                {
                    _log.Error(message: "Referral lead has invalid phone number.",
                        context: $"agentId: {referralLead.AgentId}");

                    throw new InvalidPhoneNumberException();
                }

                var phoneNumberHash = phoneNumberE164.ToSha256Hash();

                var referralLeadEncrypted = new ReferralLeadEncrypted
                {
                    PhoneCountryCodeId = referralLead.PhoneCountryCodeId,
                    PhoneNumberHash = phoneNumberHash,
                    EmailHash = emailHash,
                    AgentId = referralLead.AgentId,
                    AgentSalesforceId = agent.SalesforceId,
                    ConfirmationToken = GenerateConfirmationToken(),
                    // State is automatically approved
                    State = ReferralLeadState.Pending
                };

                referralLeadEncrypted = await _referralLeadRepository.CreateAsync(referralLeadEncrypted);

                createdReferralLead = _mapper.Map<ReferralLead>(referralLeadEncrypted);

                createdReferralLead.FirstName = referralLead.FirstName;
                createdReferralLead.LastName = referralLead.LastName;
                createdReferralLead.Email = referralLead.Email;
                createdReferralLead.PhoneNumber = phoneNumberE164;
                createdReferralLead.Note = referralLead.Note;

                var response = await _customerProfileClient.ReferralLeadProfiles.AddAsync(new ReferralLeadProfileRequest
                {
                    ReferralLeadId = referralLeadEncrypted.Id,
                    FirstName = createdReferralLead.FirstName,
                    LastName = createdReferralLead.LastName,
                    PhoneNumber = createdReferralLead.PhoneNumber,
                    Email = createdReferralLead.Email,
                    Note = createdReferralLead.Note
                });

                if (response.ErrorCode != ReferralLeadProfileErrorCodes.None)
                {
                    _log.Error(message: "An error occurred while creating referral lead profile",
                        context: $"referralLeadId: {createdReferralLead.Id}");
                }

                await _notificationPublisherService.LeadConfirmRequestAsync(
                    referralLead.AgentId.ToString(),
                    phoneNumberE164, 
                    referralLeadEncrypted.ConfirmationToken);

            }
            catch (Exception e)
            {
                _log.Error("Demo service failed to process the request.", e);
            }

            return createdReferralLead;
        }

        public async Task<ReferralLead> GetReferralLeadByConfirmationTokenAsync(string confirmationToken)
        {
            var result = await _referralLeadRepository.GetByConfirmationTokenAsync(confirmationToken);

            return await DecryptAsync(result);
        }

        public async Task ConfirmReferralLeadAsync(string confirmationToken)
        {
            try
            {
                var referralLeadEncrypted = await _referralLeadRepository.GetByConfirmationTokenAsync(confirmationToken);

                if (referralLeadEncrypted == null)
                {
                    throw new ReferralDoesNotExistException(
                        $"Referral Lead with confirmation token '{confirmationToken}' does not exist.");
                }

                if (referralLeadEncrypted.State == ReferralLeadState.Approved)
                {
                   return;
                }

                var referralLead = await DecryptAsync(referralLeadEncrypted);

                // publish confirmed event
                await PublishLeadChangeStateEvent(referralLead.Id.ToString(),
                    Contract.Enums.ReferralLeadState.Confirmed);

                // Publish approved event
                await Task.Delay(_delay);
                await PublishLeadApproved(referralLead, referralLeadEncrypted);

                // publish otp event
                await Task.Delay(_delay);
                await PublishOTP(referralLead);

                // publish commission event
                await Task.Delay(_delay);
                await PublishCommissionEvents(referralLead);

                referralLeadEncrypted.State = ReferralLeadState.Approved;

                await _referralLeadRepository.UpdateAsync(referralLeadEncrypted);
            }
            catch (Exception e)
            {
                _log.Error("Demo service failed to process the request.", e);
            }
        }

        private async Task PublishLeadApproved(ReferralLead referralLead, ReferralLeadEncrypted referralLeadEncrypted)
        {
            await PublishLeadChangeStateEvent(referralLead.Id.ToString(),
                Contract.Enums.ReferralLeadState.Approved);

            await _propertyLeadApprovedReferralPublisher.PublishAsync(new PropertyLeadApprovedReferralEvent
            {
                ReferrerId = referralLeadEncrypted.AgentId.ToString(),
                TimeStamp = DateTime.UtcNow,
                ReferralId = referralLeadEncrypted.Id.ToString()
            });
        }

        private async Task PublishCommissionEvents(ReferralLead createdReferralLead)
        {
            var propertyPurchase = new PropertyPurchase
            {
                CurrencyCode = "AED",
                DiscountAmount = 1,
                NetPropertyPrice = 1,
                ReferralLeadId = createdReferralLead.Id,
                SellingPropertyPrice = 1,
                Timestamp = DateTime.UtcNow,
                VatAmount = 1
            };

            await _propertyPurchaseRepository.InsertAsync(propertyPurchase);

            var lead = await _referralLeadRepository.GetAsync(propertyPurchase.ReferralLeadId);
        }

        private async Task PublishOTP(ReferralLead createdReferralLead)
        {
            var offerToPurchase = new OfferToPurchase
            {
                CurrencyCode = "AED",
                DiscountAmount = 1,
                NetPropertyPrice = 1,
                ReferId = createdReferralLead.Id,
                SellingPropertyPrice = 1,
                Timestamp = DateTime.UtcNow,
                VatAmount = 1
            };
            await _offerToPurchasePurchaseRepository.InsertAsync(offerToPurchase);

            var lead = await _referralLeadRepository.GetAsync(offerToPurchase.ReferId);

            await _offerToPurchasePublisher.PublishAsync(new OfferToPurchaseByLeadEvent
            {
                AgentId = lead.AgentId.ToString(),
                TimeStamp = offerToPurchase.Timestamp,
                CurrencyCode = offerToPurchase.CurrencyCode,
                VatAmount = offerToPurchase.VatAmount,
                DiscountAmount = offerToPurchase.DiscountAmount,
                NetPropertyPrice = offerToPurchase.NetPropertyPrice,
                SellingPropertyPrice = offerToPurchase.SellingPropertyPrice,
                ReferralId = lead.Id.ToString()
            });
        }

        private string GenerateConfirmationToken()
        {
            return _hashingManager
                .GenerateBase(Guid.NewGuid().ToString())
                .Substring(
                    0,
                    _settingsService.GetLeadConfirmationTokenLength()) + "_demo";
        }

        private Task PublishLeadChangeStateEvent(string leadId, Contract.Enums.ReferralLeadState state)
        {
            return _leadStateChangedPublisher.PublishAsync(new LeadStateChangedEvent
            {
                LeadId = leadId,
                State = state,
                TimeStamp = DateTime.UtcNow
            });
        }

        private async Task<ReferralLead> DecryptAsync(ReferralLeadEncrypted referralLeadEncrypted)
        {
            var referralLead = _mapper.Map<ReferralLead>(referralLeadEncrypted);

            await LoadSensitiveDataAsync(referralLead);

            return referralLead;
        }

        private async Task LoadSensitiveDataAsync(ReferralLead referralLead)
        {
            var response = await _customerProfileClient.ReferralLeadProfiles
                .GetByIdAsync(referralLead.Id);

            if (response.ErrorCode != ReferralLeadProfileErrorCodes.None)
            {
                _log.Error(message: "An error occurred while getting referral lead profile",
                    context: $"referralLeadId: {referralLead.Id}");
            }
            else
            {
                referralLead.FirstName = response.Data.FirstName;
                referralLead.LastName = response.Data.LastName;
                referralLead.Email = response.Data.Email;
                referralLead.PhoneNumber = response.Data.PhoneNumber;
                referralLead.Note = response.Data.Note;
            }
        }
    }
}
