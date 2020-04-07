//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoMapper;
//using Common.Log;
//using Falcon.Common;
//using Lykke.Common.Log;
//using Lykke.RabbitMqBroker.Publisher;
//using Lykke.Service.AgentManagement.Client;
//using Lykke.Service.AgentManagement.Client.Models.Agents;
//using Lykke.Service.Campaign.Client;
//using Lykke.Service.CustomerProfile.Client;
//using Lykke.Service.CustomerProfile.Client.Models.Enums;
//using Lykke.Service.CustomerProfile.Client.Models.Requests;
//using Lykke.Service.Dictionaries.Client;
//using MAVN.Service.Referral.Contract.Events;
//using MAVN.Service.Referral.Domain.Entities;
//using MAVN.Service.Referral.Domain.Exceptions;
//using MAVN.Service.Referral.Domain.Managers;
//using MAVN.Service.Referral.Domain.Models;
//using MAVN.Service.Referral.Domain.Repositories;
//using MAVN.Service.Referral.Domain.Services;
//using MAVN.Service.Referral.DomainServices.Extensions;

//namespace MAVN.Service.Referral.DomainServices.Services
//{
//    public class ReferralLeadService : IReferralLeadService
//    {
//        private const string EstatePurchaseConditionName = "estate-lead-referral";

//        private readonly IStakeService _stakeService;
//        private readonly IMAVNPropertyIntegrationClient _propertyIntegrationClient;
//        private readonly IRabbitPublisher<PropertyLeadApprovedReferralEvent> _propertyLeadApprovedReferralPublisher;
//        private readonly IReferralLeadRepository _referralLeadRepository;
//        private readonly INotificationPublisherService _notificationPublisherService;
//        private readonly ISettingsService _settingsService;
//        private readonly IHashingManager _hashingManager;
//        private readonly IAgentManagementClient _agentManagementClient;
//        private readonly ICustomerProfileClient _customerProfileClient;
//        private readonly IDictionariesClient _dictionariesClient;
//        private readonly IPropertyPurchaseRepository _propertyPurchaseRepository;
//        private readonly IRabbitPublisher<LeadStateChangedEvent> _leadStateChangedPublisher;
//        private readonly IMapper _mapper;
//        private readonly ILog _log;

//        public ReferralLeadService(
//            IStakeService stakeService,
//            IMAVNPropertyIntegrationClient propertyIntegrationClient,
//            IRabbitPublisher<PropertyLeadApprovedReferralEvent> propertyLeadApprovedReferralPublisher,
//            IReferralLeadRepository referralLeadRepository,
//            INotificationPublisherService notificationPublisherService,
//            ISettingsService settingsService,
//            IHashingManager hashingManager,
//            IAgentManagementClient agentManagementClient,
//            ICustomerProfileClient customerProfileClient,
//            IDictionariesClient dictionariesClient,
//            IPropertyPurchaseRepository propertyPurchaseRepository,
//            IRabbitPublisher<LeadStateChangedEvent> leadStateChangedPublisher,
//            IMapper mapper,
//            ILogFactory logFactory)
//        {
//            _stakeService = stakeService;
//            _propertyIntegrationClient = propertyIntegrationClient;
//            _propertyLeadApprovedReferralPublisher = propertyLeadApprovedReferralPublisher;
//            _referralLeadRepository = referralLeadRepository;
//            _notificationPublisherService = notificationPublisherService;
//            _settingsService = settingsService;
//            _hashingManager = hashingManager;
//            _agentManagementClient = agentManagementClient;
//            _customerProfileClient = customerProfileClient;
//            _dictionariesClient = dictionariesClient;
//            _propertyPurchaseRepository = propertyPurchaseRepository;
//            _leadStateChangedPublisher = leadStateChangedPublisher
//                ?? throw new ArgumentNullException(nameof(propertyLeadApprovedReferralPublisher));
//            _mapper = mapper;
//            _log = logFactory.CreateLog(this);
//        }

//        public async Task<ReferralLead> CreateReferralLeadAsync(ReferralLead referralLead)
//        {
//            var agentCustomer = await _customerProfileClient.CustomerProfiles
//                .GetByCustomerIdAsync(referralLead.AgentId.ToString(), false, false);

//            if (agentCustomer.ErrorCode == CustomerProfileErrorCodes.CustomerProfileDoesNotExist)
//            {
//                throw new CustomerDoesNotExistException($"Customer '{referralLead.AgentId}' does not exist.");
//            }

//            var agent = await _agentManagementClient.Agents.GetByCustomerIdAsync(referralLead.AgentId);

//            if (agent == null || agent.Status != AgentStatus.ApprovedAgent)
//            {
//                throw new CustomerNotApprovedAgentException("Customer isn't an approved agent.");
//            }

//            var countryPhoneCode = await _dictionariesClient.Salesforce
//               .GetCountryPhoneCodeByIdAsync(referralLead.PhoneCountryCodeId);

//            if (countryPhoneCode == null)
//            {
//                throw new CountryCodeDoesNotExistException(
//                    $"Country information for Country code id '{referralLead.PhoneCountryCodeId}' does not exist.");
//            }

//            if (agentCustomer.Profile.Email == referralLead.Email ||
//                agentCustomer.Profile.CountryPhoneCodeId == referralLead.PhoneCountryCodeId &&
//                (agentCustomer.Profile.PhoneNumber == referralLead.PhoneNumber ||
//                 agentCustomer.Profile.ShortPhoneNumber == referralLead.PhoneNumber))
//            {
//                throw new ReferYourselfException("You can not refer yourself as lead.");
//            }

//            var emailHash = referralLead.Email.ToSha256Hash();

//            var phoneNumberE164 = PhoneUtils.GetE164FormattedNumber(referralLead.PhoneNumber, countryPhoneCode.IsoCode);

//            if (string.IsNullOrEmpty(phoneNumberE164))
//            {
//                _log.Error(message: "Referral lead has invalid phone number.",
//                    context: $"agentId: {referralLead.AgentId}");

//                throw new InvalidPhoneNumberException();
//            }

//            var phoneNumberHash = phoneNumberE164.ToSha256Hash();

//            if (await ReferralLeadAlreadyExistsAsync(referralLead.AgentId, emailHash, referralLead.PhoneCountryCodeId, phoneNumberHash))
//            {
//                throw new ReferralAlreadyExistException("Lead with the same Phone or Email is already referred from you.");
//            }

//            if (await ConfirmedLeadExistsAsync(emailHash, referralLead.PhoneCountryCodeId, phoneNumberHash))
//            {
//                await _notificationPublisherService.LeadAlreadyConfirmedAsync(
//                    referralLead.AgentId.ToString(), referralLead.FirstName, referralLead.LastName, phoneNumberE164);

//                throw new ReferralLeadAlreadyConfirmedException("Lead with the same Phone or Email already confirmed.");
//            }

//            if (agent == null || agent.Status != AgentStatus.ApprovedAgent)
//            {
//                throw new CustomerNotApprovedAgentException("Customer isn't an approved agent.");
//            }

//            var referralStake = await _stakeService.GetReferralStake(referralLead.CampaignId, EstatePurchaseConditionName);
            
//            var referralLeadEncrypted = new ReferralLeadEncrypted
//            {
//                Id = Guid.NewGuid(),
//                PhoneCountryCodeId = referralLead.PhoneCountryCodeId,
//                PhoneNumberHash = phoneNumberHash,
//                EmailHash = emailHash,
//                AgentId = referralLead.AgentId,
//                AgentSalesforceId = agent.SalesforceId,
//                ConfirmationToken = GenerateConfirmationToken(),
//                State = ReferralLeadState.Pending,
//                CampaignId = referralLead.CampaignId,
//                StakeEnabled = referralStake != null
//            };

//            if (referralStake != null)
//            {
//                await _stakeService.SetStake(
//                    referralStake,
//                    referralLead.AgentId.ToString("D"),
//                    referralLeadEncrypted.Id.ToString("D"));
//            }

//            referralLeadEncrypted = await _referralLeadRepository.CreateAsync(referralLeadEncrypted);

//            var createdReferralLead = _mapper.Map<ReferralLead>(referralLeadEncrypted);

//            createdReferralLead.FirstName = referralLead.FirstName;
//            createdReferralLead.LastName = referralLead.LastName;
//            createdReferralLead.Email = referralLead.Email;
//            createdReferralLead.PhoneNumber = phoneNumberE164;
//            createdReferralLead.Note = referralLead.Note;

//            var response = await _customerProfileClient.ReferralLeadProfiles.AddAsync(new ReferralLeadProfileRequest
//            {
//                ReferralLeadId = referralLeadEncrypted.Id,
//                FirstName = createdReferralLead.FirstName,
//                LastName = createdReferralLead.LastName,
//                PhoneNumber = createdReferralLead.PhoneNumber,
//                Email = createdReferralLead.Email,
//                Note = createdReferralLead.Note
//            });

//            if (response.ErrorCode != ReferralLeadProfileErrorCodes.None)
//            {
//                _log.Error(message: "An error occurred while creating referral lead profile",
//                    context: $"referralLeadId: {createdReferralLead.Id}");
//            }

//            await _notificationPublisherService.LeadConfirmRequestAsync(referralLead.AgentId.ToString(),
//                phoneNumberE164, referralLeadEncrypted.ConfirmationToken);

//            await PublishLeadChangeStateEvent(referralLeadEncrypted.Id.ToString(),
//                _mapper.Map<Contract.Enums.ReferralLeadState>(referralLead.State));

//            return createdReferralLead;
//        }

//        public async Task<IReadOnlyList<ReferralLeadWithDetails>> GetReferralLeadsForReferrerAsync(
//            Guid referrerId,
//            Guid? campaignId,
//            IEnumerable<ReferralLeadState> states)
//        {
//            var referralLeadsEncryptedWithDetails = await _referralLeadRepository.GetForReferrerAsync(
//                referrerId,
//                campaignId,
//                states);

//            var referralLeadsWithDetails = new List<ReferralLeadWithDetails>();

//            foreach (var referralLeadEncryptedWithDetails in referralLeadsEncryptedWithDetails)
//            {
//                var referralLeadWithDetails = _mapper.Map<ReferralLeadWithDetails>(referralLeadEncryptedWithDetails);

//                await LoadSensitiveDataAsync(referralLeadWithDetails);

//                referralLeadsWithDetails.Add(referralLeadWithDetails);
//            }

//            return referralLeadsWithDetails;
//        }

//        public async Task<IReadOnlyList<ReferralLead>> GetApprovedLeadsAsync()
//        {
//            var referralLeadsEncrypted = await _referralLeadRepository.GetApprovedAsync();

//            return await DecryptAsync(referralLeadsEncrypted);
//        }

//        public async Task<ReferralLead> ConfirmReferralLeadAsync(string confirmationToken)
//        {
//            var referralLeadEncrypted = await _referralLeadRepository.GetByConfirmationTokenAsync(confirmationToken);

//            if (referralLeadEncrypted == null)
//            {
//                throw new ReferralDoesNotExistException(
//                    $"Referral Lead with confirmation token '{confirmationToken}' does not exist.");
//            }

//            if (referralLeadEncrypted.State != ReferralLeadState.Pending)
//            {
//                return await DecryptAsync(referralLeadEncrypted);
//            }

//            var countryPhoneCode = await _dictionariesClient.Salesforce
//                .GetCountryPhoneCodeByIdAsync(referralLeadEncrypted.PhoneCountryCodeId);

//            if (countryPhoneCode == null)
//            {
//                throw new CountryCodeDoesNotExistException(
//                    $"Country information for Country code id '{referralLeadEncrypted.PhoneCountryCodeId}' does not exist.");
//            }

//            if (await ConfirmedLeadExistsAsync(referralLeadEncrypted.EmailHash,
//                referralLeadEncrypted.PhoneCountryCodeId, referralLeadEncrypted.PhoneNumberHash))
//            {
//                throw new ReferralAlreadyConfirmedException("Lead with the same Phone or Email already confirmed.");
//            }

//            var referralLead = await DecryptAsync(referralLeadEncrypted);

//            LeadRegisterResponseModel result;

//            try
//            {
//                var phoneNumber = RemoveCountryCode(referralLead.PhoneNumber, countryPhoneCode.IsoCode);

//                result = await _propertyIntegrationClient.Api.RegisterLeadAsync(new LeadRegisterRequestModel
//                {
//                    FirstName = referralLead.FirstName,
//                    LastName = referralLead.LastName,
//                    PhoneCountryName = countryPhoneCode.CountryName,
//                    PhoneCountryCode = countryPhoneCode.Code,
//                    PhoneNumber = phoneNumber,
//                    AgentSalesforceId = referralLead.AgentSalesforceId,
//                    Email = referralLead.Email,
//                    LeadNote = referralLead.Note,
//                    ReferId = referralLead.Id.ToString()
//                });
//            }
//            catch (Exception e)
//            {
//                _log.Error(message: "Confirming lead failed", exception: e);

//                throw new ReferralLeadConfirmationFailedException(
//                    $"Confirmation for property referral '{referralLead.Id}' failed.");
//            }

//            if (result.Status.ToLower() != "success")
//            {
//                _log.Warning($"Confirming lead failed with status '{result.Status}'", context: confirmationToken);
//            }

//            referralLeadEncrypted.SalesforceId = result.SalesforceId;
//            referralLeadEncrypted.ResponseStatus = result.Status;
//            referralLeadEncrypted.State = ReferralLeadState.Confirmed;

//            referralLeadEncrypted = await _referralLeadRepository.UpdateAsync(referralLeadEncrypted);

//            _mapper.Map(referralLeadEncrypted, referralLead);

//            await _notificationPublisherService.LeadSuccessfullyConfirmedAsync(referralLead.AgentId.ToString(),
//                referralLead.FirstName, referralLead.LastName, referralLead.PhoneNumber);

//            await PublishLeadChangeStateEvent(referralLead.Id.ToString(),
//                _mapper.Map<Contract.Enums.ReferralLeadState>(referralLead.State));

//            return referralLead;
//        }

//        public async Task<ReferralLead> ApproveReferralLeadAsync(Guid referralId, DateTime timestamp)
//        {
//            var referralLeadEncrypted = await _referralLeadRepository.GetAsync(referralId);

//            if (referralLeadEncrypted == null)
//            {
//                throw new ReferralDoesNotExistException($"Referral Lead with Id '{referralId}' does not exist.");
//            }

//            if (referralLeadEncrypted.State != ReferralLeadState.Confirmed)
//            {
//                throw new InvalidOperationException("Referral should be in status Confirmed.");
//            }

//            referralLeadEncrypted.State = ReferralLeadState.Approved;

//            referralLeadEncrypted = await _referralLeadRepository.UpdateAsync(referralLeadEncrypted);

//            await _propertyLeadApprovedReferralPublisher.PublishAsync(new PropertyLeadApprovedReferralEvent
//            {
//                ReferrerId = referralLeadEncrypted.AgentId.ToString(),
//                TimeStamp = timestamp,
//                StakedCampaignId = referralLeadEncrypted.CampaignId,
//                ReferralId = referralLeadEncrypted.Id.ToString()
//            });

//            await PublishLeadChangeStateEvent(referralLeadEncrypted.Id.ToString(),
//                _mapper.Map<Contract.Enums.ReferralLeadState>(referralLeadEncrypted.State));

//            return await DecryptAsync(referralLeadEncrypted);
//        }

//        public async Task<ReferralLead> RejectReferralLeadAsync(Guid referralId, DateTime timestamp)
//        {
//            var referralLeadEncrypted = await _referralLeadRepository.GetAsync(referralId);

//            if (referralLeadEncrypted == null)
//            {
//                throw new ReferralDoesNotExistException($"Referral Lead with Id '{referralId}' does not exist.");
//            }

//            if (referralLeadEncrypted.State != ReferralLeadState.Confirmed)
//            {
//                throw new InvalidOperationException("Referral should be in status Confirmed.");
//            }

//            referralLeadEncrypted.State = ReferralLeadState.Rejected;

//            referralLeadEncrypted = await _referralLeadRepository.UpdateAsync(referralLeadEncrypted);

//            await PublishLeadChangeStateEvent(referralLeadEncrypted.Id.ToString(),
//                _mapper.Map<Contract.Enums.ReferralLeadState>(referralLeadEncrypted.State));

//            return await DecryptAsync(referralLeadEncrypted);
//        }

//        public async Task<LeadStatisticModel> GetStatistic()
//        {
//            var numOfLeads = _referralLeadRepository.GetCountAsync();
//            var approvedLeads = _referralLeadRepository.GetCountAsync(ReferralLeadState.Approved);
//            var uniqueCompletedLeadCount = _propertyPurchaseRepository.GetUniqueLeadCount();

//            await Task.WhenAll(numOfLeads, approvedLeads, uniqueCompletedLeadCount);

//            var statistic = new LeadStatisticModel
//            {
//                NumberOfLeads = numOfLeads.Result,
//                NumberOfApprovedLeads = approvedLeads.Result,
//                NumberOfUniqueCompletedLeads = uniqueCompletedLeadCount.Result
//            };

//            return statistic;
//        }

//        public async Task<Dictionary<Guid, int>> GetApprovedReferralsCountByAgentsAsync(List<Guid> agentIds)
//        {
//            return await _referralLeadRepository.GetApprovedReferralsCountByAgentsAsync(agentIds);
//        }

//        public async Task<IReadOnlyList<ReferralLeadWithDetails>> GetReferralLeadsByReferralIdsAsync(List<Guid> referralIds)
//        {
//            var referralLeadsEncryptedWithDetails = await _referralLeadRepository.GetByReferralIdsAsync(referralIds);

//            var referralLeadsWithDetails = new List<ReferralLeadWithDetails>();

//            foreach (var referralLeadEncryptedWithDetails in referralLeadsEncryptedWithDetails)
//            {
//                var referralLeadWithDetails = _mapper.Map<ReferralLeadWithDetails>(referralLeadEncryptedWithDetails);

//                await LoadSensitiveDataAsync(referralLeadWithDetails);

//                referralLeadsWithDetails.Add(referralLeadWithDetails);
//            }

//            return referralLeadsWithDetails;
//        }

//        private async Task<IReadOnlyList<ReferralLead>> DecryptAsync(
//            IEnumerable<ReferralLeadEncrypted> referralLeadsEncrypted)
//        {
//            var referralLeads = new List<ReferralLead>();

//            foreach (var referralLeadEncrypted in referralLeadsEncrypted)
//            {
//                var referralLead = await DecryptAsync(referralLeadEncrypted);

//                referralLeads.Add(referralLead);
//            }

//            return referralLeads;
//        }

//        private async Task<ReferralLead> DecryptAsync(ReferralLeadEncrypted referralLeadEncrypted)
//        {
//            var referralLead = _mapper.Map<ReferralLead>(referralLeadEncrypted);

//            await LoadSensitiveDataAsync(referralLead);

//            return referralLead;
//        }

//        private async Task LoadSensitiveDataAsync(ReferralLead referralLead)
//        {
//            var response = await _customerProfileClient.ReferralLeadProfiles
//                .GetByIdAsync(referralLead.Id);

//            if (response.ErrorCode != ReferralLeadProfileErrorCodes.None)
//            {
//                _log.Error(message: "An error occurred while getting referral lead profile",
//                    context: $"referralLeadId: {referralLead.Id}");
//            }
//            else
//            {
//                referralLead.FirstName = response.Data.FirstName;
//                referralLead.LastName = response.Data.LastName;
//                referralLead.Email = response.Data.Email;
//                referralLead.PhoneNumber = response.Data.PhoneNumber;
//                referralLead.Note = response.Data.Note;
//            }
//        }

//        private string GenerateConfirmationToken()
//        {
//            return _hashingManager
//                .GenerateBase(Guid.NewGuid().ToString())
//                .Substring(
//                    0,
//                    _settingsService.GetLeadConfirmationTokenLength());
//        }

//        private async Task<bool> ConfirmedLeadExistsAsync(string emailHash, int countryCodeId, string phoneHash)
//        {
//            var leadByEmailHashTask = _referralLeadRepository.GetByEmailHashAsync(emailHash);
//            var leadByPhoneHashTask = _referralLeadRepository.GetByPhoneNumberHashAsync(countryCodeId, phoneHash);

//            await Task.WhenAll(leadByEmailHashTask, leadByPhoneHashTask);

//            var leadsByEmail = leadByEmailHashTask.Result;
//            var leadsByPhone = leadByPhoneHashTask.Result;

//            if (leadsByEmail.Any(x => x.State != ReferralLeadState.Pending))
//            {
//                return true;
//            }

//            if (leadsByPhone.Any(x => x.State != ReferralLeadState.Pending))
//            {
//                return true;
//            }

//            return false;
//        }

//        private async Task<bool> ReferralLeadAlreadyExistsAsync(Guid agentId, string emailHash, int countryCodeId, string phoneHash)
//        {
//            var leadByEmailTask = _referralLeadRepository.GetByEmailHashAsync(emailHash);
//            var leadByPhoneTask = _referralLeadRepository.GetByPhoneNumberHashAsync(countryCodeId, phoneHash);

//            await Task.WhenAll(leadByEmailTask, leadByPhoneTask);

//            var leadsByEmail = leadByEmailTask.Result;
//            var leadsByPhone = leadByPhoneTask.Result;

//            return leadsByEmail.Any(r => r.AgentId == agentId)
//                   || leadsByPhone.Any(r => r.AgentId == agentId);
//        }

//        private Task PublishLeadChangeStateEvent(string leadId, Contract.Enums.ReferralLeadState state)
//        {
//            return _leadStateChangedPublisher.PublishAsync(new LeadStateChangedEvent
//            {
//                LeadId = leadId,
//                State = state,
//                TimeStamp = DateTime.UtcNow
//            });
//        }

//        // NOTE: Salesforce requires that we are sending phone number without ISO code in it
//        private static string RemoveCountryCode(string phoneNumber, string countryCode)
//        {
//            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(countryCode))
//            {
//                return null;
//            }

//            var index = phoneNumber.IndexOf(countryCode, StringComparison.InvariantCultureIgnoreCase);

//            if (index == -1)
//            {
//                return phoneNumber;
//            }

//            return phoneNumber.Substring(index + countryCode.Length);
//        }
//    }
//}

