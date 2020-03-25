using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.Campaign.Client;
using Lykke.Service.Campaign.Client.Models.Enums;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CurrencyConvertor.Client.Models.Enums;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.CustomerProfile.Client.Models.Enums;
using Lykke.Service.CustomerProfile.Client.Models.Requests;
using Lykke.Service.PartnerManagement.Client;
using Lykke.Service.Referral.Contract.Events;
using Lykke.Service.Referral.Domain.Exceptions;
using Lykke.Service.Referral.Domain.Managers;
using Lykke.Service.Referral.Domain.Models;
using Lykke.Service.Referral.Domain.Repositories;
using Lykke.Service.Referral.Domain.Services;
using Lykke.Service.Referral.DomainServices.Extensions;

namespace Lykke.Service.Referral.DomainServices.Services
{
    public class ReferralHotelsService : IReferralHotelsService
    {
        private readonly string _globalBaseCurrencyCode;
        private readonly IStakeService _stakeService;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly ICurrencyConvertorClient _currencyConverterClient;
        private readonly ICampaignClient _campaignClient;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IRabbitPublisher<HotelReferralUsedEvent> _rabbitPublisher;
        private readonly INotificationPublisherService _notificationPublisherService;
        private readonly IReferralHotelsRepository _referralHotelsRepository;
        private readonly ISettingsService _settingsService;
        private readonly IHashingManager _hashingManager;
        private readonly TimeSpan _referralExpirationPeriod;
        private readonly TimeSpan _referralLimitPeriod;
        private readonly int _referralLimit;
        private readonly ILog _log;
        private readonly IMapper _mapper;
        private const string ConditionType = "hotel-stay-referral";

        public ReferralHotelsService(
            IStakeService stakeService,
            ICustomerProfileClient customerProfileClient,
            ICurrencyConvertorClient currencyConverterClient,
            ICampaignClient campaignClient,
            IPartnerManagementClient partnerManagementClient,
            IRabbitPublisher<HotelReferralUsedEvent> rabbitPublisher,
            INotificationPublisherService notificationPublisherService,
            IReferralHotelsRepository referralHotelsRepository,
            ISettingsService settingsService,
            IHashingManager hashingManager,
            TimeSpan referralExpirationPeriod,
            TimeSpan referralLimitPeriod,
            int referralLimit,
            IMapper mapper,
            string globalBaseCurrencyCode,
            ILogFactory logFactory)
        {
            _stakeService = stakeService;
            _customerProfileClient = customerProfileClient;
            _currencyConverterClient = currencyConverterClient;
            _campaignClient = campaignClient;
            _partnerManagementClient = partnerManagementClient;
            _rabbitPublisher = rabbitPublisher;
            _notificationPublisherService = notificationPublisherService;
            _referralHotelsRepository = referralHotelsRepository;
            _settingsService = settingsService;
            _hashingManager = hashingManager;
            _referralExpirationPeriod = referralExpirationPeriod;
            _referralLimitPeriod = referralLimitPeriod;
            _referralLimit = referralLimit;
            _mapper = mapper;
            _globalBaseCurrencyCode = globalBaseCurrencyCode;
            _log = logFactory.CreateLog(this);
        }

        public async Task<ReferralHotel> CreateAsync(
            string email,
            string referrerId,
            Guid? campaignId,
            int phoneCountryCodeId,
            string phoneNumber,
            string fullName)
        {
            if (!await CustomerExistsAsync(referrerId))
            {
                throw new CustomerDoesNotExistException();
            }
            
            if (await CustomerReferencesHimselfAsync(referrerId, email))
            {
                throw new ReferYourselfException();
            }

            if (await LimitExceededAsync(referrerId))
            {
                throw new ReferralHotelLimitExceededException();
            }

            var emailHash = email.ToSha256Hash();

            if (await ConfirmedReferralExistsAsync(emailHash))
            {
                throw new ReferralAlreadyConfirmedException();
            }

            if (await ExpiredReferralExistsAsync(emailHash))
            {
                throw new ReferralExpiredException(emailHash);
            }

            if (await ReferralByThisReferrerExistsAsync(referrerId, emailHash))
            {
                throw new ReferralAlreadyExistException();
            }

            var referralStake = await _stakeService.GetReferralStake(campaignId, ConditionType);

            Guid? partnerId = null;

            if (campaignId.HasValue)
            {
                var campaign = await _campaignClient.Campaigns.GetByIdAsync(campaignId.Value.ToString("D"));

                if (campaign.ErrorCode == CampaignServiceErrorCodes.EntityNotFound)
                {
                    throw new CampaignNotFoundException($"Campaign with id '{campaignId.Value}' was not found.");
                }

                var condition = campaign.Conditions.FirstOrDefault(c => c.Type == ConditionType);

                if (condition != null)
                {
                    if (condition.PartnerIds.Length > 1)
                    {
                        // Currently we only support one partner
                        _log.Warning($"Campaign condition with multiple partners found.", context: campaign);
                    }

                    if (condition.PartnerIds.Any())
                    {
                        partnerId = condition.PartnerIds.First();
                    }
                }
            }

            var creationDateTime = DateTime.UtcNow;

            var referralHotelEncrypted = new ReferralHotelEncrypted
            {
                Id = Guid.NewGuid().ToString("D"),
                EmailHash = emailHash,
                FullNameHash = fullName.ToSha256Hash(),
                PhoneCountryCodeId = phoneCountryCodeId,
                PhoneNumberHash = phoneNumber.ToSha256Hash(),
                ReferrerId = referrerId,
                ConfirmationToken = GenerateConfirmationToken(),
                CreationDateTime = creationDateTime,
                ExpirationDateTime = creationDateTime + _referralExpirationPeriod,
                CampaignId = campaignId,
                PartnerId = partnerId?.ToString("D"),
                StakeEnabled = referralStake != null
            };

            if (referralStake != null)
            {
                await _stakeService.SetStake(
                    referralStake,
                    referralHotelEncrypted.ReferrerId,
                    referralHotelEncrypted.Id);
            }

            referralHotelEncrypted = await _referralHotelsRepository.CreateAsync(referralHotelEncrypted);

            var createdReferralHotel = _mapper.Map<ReferralHotel>(referralHotelEncrypted);

            createdReferralHotel.Email = email;
            createdReferralHotel.PhoneNumber = phoneNumber;
            createdReferralHotel.FullName = fullName;

            var response = await _customerProfileClient.ReferralHotelProfiles.AddAsync(new ReferralHotelProfileRequest
            {
                ReferralHotelId = Guid.Parse(createdReferralHotel.Id),
                Email = createdReferralHotel.Email,
                PhoneNumber = createdReferralHotel.PhoneNumber,
                Name = createdReferralHotel.FullName
            });

            if (response.ErrorCode != ReferralHotelProfileErrorCodes.None)
            {
                _log.Error(message: "An error occurred while creating referral hotel profile",
                    context: $"referralHotelId: {createdReferralHotel.Id}");
            }

            await _notificationPublisherService.HotelReferralConfirmRequestAsync(createdReferralHotel.ReferrerId,
                createdReferralHotel.Email, _referralExpirationPeriod, createdReferralHotel.ConfirmationToken);

            _log.Info("Referral hotel created", context: $"referralHotelId: {createdReferralHotel.Id}");

            return createdReferralHotel;
        }

        public async Task<ReferralHotel> ConfirmAsync(string confirmationToken)
        {
            var referralHotelEncrypted = await _referralHotelsRepository.GetByConfirmationTokenAsync(confirmationToken);

            if (referralHotelEncrypted == null)
            {
                throw new ReferralDoesNotExistException();
            }

            if (referralHotelEncrypted.State == ReferralHotelState.Confirmed)
            {
                return await DecryptAsync(referralHotelEncrypted);
            }

            if (await ConfirmedReferralExistsAsync(referralHotelEncrypted.EmailHash))
            {
                throw new ReferralAlreadyConfirmedException();
            }

            if (ReferralExpired(referralHotelEncrypted))
            {
                referralHotelEncrypted.State = ReferralHotelState.Expired;

                await _referralHotelsRepository.UpdateAsync(referralHotelEncrypted);

                throw new ReferralExpiredException(referralHotelEncrypted.Id);
            }

            referralHotelEncrypted.State = ReferralHotelState.Confirmed;

            referralHotelEncrypted = await _referralHotelsRepository.UpdateAsync(referralHotelEncrypted);

            _log.Info("Referral hotel confirmed", context: $"referralHotelId: {referralHotelEncrypted.Id}");

            return await DecryptAsync(referralHotelEncrypted);
        }

        public async Task<ReferralHotel> UseAsync(ReferralHotelUseModel useModel)
        {
            await VerifyPartnerIdAsync(useModel.PartnerId);

            var buyerEmailHash = useModel.BuyerEmail.ToSha256Hash();

            var referrals = await _referralHotelsRepository.GetByEmailHashAsync(buyerEmailHash, useModel.PartnerId, null);

            referrals = referrals
                .Where(x => (string.IsNullOrEmpty(x.PartnerId)) || x.PartnerId == useModel.PartnerId)
                .ToList();

            if (referrals.Any() == false)
            {
                throw new ReferralDoesNotExistException();
            }

            var referralHotelEncrypted = referrals
                .Where(x => x.State != ReferralHotelState.Pending)
                .OrderByDescending(x => x.CreationDateTime)
                .FirstOrDefault();

            // Any check guarantees there is at least 1 item
            if (referralHotelEncrypted == null)
            {
                throw new ReferralNotConfirmedException(
                    referrals
                        .OrderByDescending(x => x.CreationDateTime)
                        .First()
                        .Id);
            }

            if (referralHotelEncrypted.State == ReferralHotelState.Used)
            {
                throw new ReferralAlreadyUsedException(referralHotelEncrypted.Id);
            }

            // Used should not be set to expired
            if (referralHotelEncrypted.State == ReferralHotelState.Expired || ReferralExpired(referralHotelEncrypted))
            {
                if (referralHotelEncrypted.State != ReferralHotelState.Expired)
                {
                    referralHotelEncrypted.State = ReferralHotelState.Expired;

                    await _referralHotelsRepository.UpdateAsync(referralHotelEncrypted);
                }

                throw new ReferralExpiredException(referralHotelEncrypted.Id);
            }

            // Guard Check
            var currency = await _currencyConverterClient.Converter.ConvertAsync(useModel.CurrencyCode, _globalBaseCurrencyCode, useModel.Amount);
            if (currency.ErrorCode == ConverterErrorCode.NoRate)
            {
                throw new InvalidCurrencyException();
            }

            if (referralHotelEncrypted.StakeEnabled)
            {
                await _stakeService.ReleaseStake(referralHotelEncrypted.Id, referralHotelEncrypted.CampaignId.Value, ConditionType);
            }

            await _rabbitPublisher.PublishAsync(new HotelReferralUsedEvent
            {
                CustomerId = referralHotelEncrypted.ReferrerId,
                Amount = useModel.Amount,
                CurrencyCode = useModel.CurrencyCode,
                LocationId = useModel.Location,
                PartnerId = useModel.PartnerId,
                StakedCampaignId = referralHotelEncrypted.StakeEnabled ? referralHotelEncrypted.CampaignId : null,
                ReferralId = referralHotelEncrypted.Id
            });

            referralHotelEncrypted.State = ReferralHotelState.Used;
            referralHotelEncrypted.Location = useModel.Location;
            referralHotelEncrypted.PartnerId = useModel.PartnerId;

            referralHotelEncrypted = await _referralHotelsRepository.UpdateAsync(referralHotelEncrypted);

            _log.Info("Referral hotel used", context: $"referralHotelId: {referralHotelEncrypted.Id}");

            return await DecryptAsync(referralHotelEncrypted);
        }

        private async Task VerifyPartnerIdAsync(string partnerId)
        {
            if (string.IsNullOrEmpty(partnerId))
            {
                return;
            }

            if (!Guid.TryParse(partnerId, out var partnerIdGuid))
            {
                throw new PartnerNotFoundException($"Partner with invalid id '{partnerId}' cannot be found.");
            }

            var partner = await _partnerManagementClient.Partners.GetByIdAsync(partnerIdGuid);

            if (partner == null)
            {
                throw new PartnerNotFoundException($"Partner with id '{partnerId}' cannot be found.");
            }
        }

        public async Task<IReadOnlyList<ReferralHotelWithProfile>> GetByReferrerIdAsync(
            string customerId,
            Guid? campaignId,
            IEnumerable<ReferralHotelState> states)
        {
            if (!await CustomerExistsAsync(customerId))
            {
                throw new CustomerDoesNotExistException();
            }

            var referralHotelsEncrypted = await _referralHotelsRepository.GetByReferrerIdAsync(customerId, campaignId, states);

            var decrypted = await DecryptAsync(referralHotelsEncrypted);
            var result = new List<ReferralHotelWithProfile>();
            foreach (var referralHotel in decrypted)
            {
                var leadWithProfile = _mapper.Map<ReferralHotelWithProfile>(referralHotel);
                var customerResponse = await _customerProfileClient.CustomerProfiles.GetByEmailAsync(new GetByEmailRequestModel
                {
                    Email = referralHotel.Email,
                    IncludeNotVerified = false
                });
                if (customerResponse.ErrorCode == CustomerProfileErrorCodes.None)
                {
                    leadWithProfile.CustomerId = customerResponse.Profile.CustomerId;
                }
                
                var splittedName = SplitFullNameIntoNameAndSurname(referralHotel.FullName);
                    
                leadWithProfile.FirstName = splittedName[0];
                leadWithProfile.LastName = splittedName[1];
                leadWithProfile.FullName = referralHotel.FullName;
                leadWithProfile.PhoneNumber = referralHotel.PhoneNumber;

                result.Add(leadWithProfile);
            }

            return result;
        }

        public async Task<IReadOnlyList<ReferralHotel>> GetByEmailAsync(string email, string partnerId, string location)
        {
            var emailHash = email.ToSha256Hash();

            var referralHotelsEncrypted =
                await _referralHotelsRepository.GetByEmailHashAsync(emailHash, partnerId, location);

            return await DecryptAsync(referralHotelsEncrypted);
        }

        public async Task<IReadOnlyList<ReferralHotelWithProfile>> GetReferralHotelsByReferralIdsAsync(List<Guid> referralIds)
        {
            var referralHotelsEncrypted = await _referralHotelsRepository.GetByReferralIdsAsync(referralIds);

            var decrypted = await DecryptAsync(referralHotelsEncrypted);
            var result = new List<ReferralHotelWithProfile>();
            foreach (var referralHotel in decrypted)
            {
                var leadWithProfile = _mapper.Map<ReferralHotelWithProfile>(referralHotel);
                var customerResponse = await _customerProfileClient.CustomerProfiles.GetByEmailAsync(new GetByEmailRequestModel
                {
                    Email = referralHotel.Email,
                    IncludeNotVerified = false
                });
                if (customerResponse.ErrorCode == CustomerProfileErrorCodes.None)
                {
                    leadWithProfile.CustomerId = customerResponse.Profile.CustomerId;
                    leadWithProfile.FirstName = customerResponse.Profile.FirstName;
                    leadWithProfile.LastName = customerResponse.Profile.LastName;
                }

                result.Add(leadWithProfile);
            }

            return result;
        }

        private async Task<IReadOnlyList<ReferralHotel>> DecryptAsync(
            IEnumerable<ReferralHotelEncrypted> referralHotelsEncrypted)
        {
            var referralHotels = new List<ReferralHotel>();

            foreach (var referralHotelEncrypted in referralHotelsEncrypted)
            {
                var referralHotel = await DecryptAsync(referralHotelEncrypted);

                referralHotels.Add(referralHotel);
            }

            return referralHotels;
        }

        private async Task<ReferralHotel> DecryptAsync(ReferralHotelEncrypted referralHotelEncrypted)
        {
            var referralHotel = _mapper.Map<ReferralHotel>(referralHotelEncrypted);

            var response = await _customerProfileClient.ReferralHotelProfiles
                .GetByIdAsync(Guid.Parse(referralHotelEncrypted.Id));

            if (response.ErrorCode != ReferralHotelProfileErrorCodes.None)
            {
                _log.Error(message: "An error occurred while getting referral hotel profile",
                    context: $"referralHotelId: {referralHotelEncrypted.Id}");
            }
            else
            {
                referralHotel.Email = response.Data.Email;
                referralHotel.PhoneNumber = response.Data.PhoneNumber;
                referralHotel.FullName = response.Data.Name;
            }

            return referralHotel;
        }

        private async Task<bool> CustomerExistsAsync(string customerId)
        {
            var response = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(customerId);

            return response.ErrorCode != CustomerProfileErrorCodes.CustomerProfileDoesNotExist;
        }

        private async Task<bool> CustomerReferencesHimselfAsync(string customerId, string email)
        {
            var response = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(customerId);

            return response.Profile.Email == email;
        }

        private async Task<bool> ConfirmedReferralExistsAsync(string emailHash)
        {
            var referralsByEmail = await _referralHotelsRepository.GetByEmailHashAsync(emailHash, null, null);

            return referralsByEmail.Any(x => x.State != ReferralHotelState.Pending &&
                                             x.State != ReferralHotelState.Used &&
                                             x.State != ReferralHotelState.Expired);
        }

        private async Task<bool> ExpiredReferralExistsAsync(string emailHash)
        {
            var referralsByEmail = await _referralHotelsRepository.GetByEmailHashAsync(emailHash, null, null);

            return referralsByEmail.Any(x => x.State == ReferralHotelState.Expired || ReferralExpired(x));
        }

        private async Task<bool> ReferralByThisReferrerExistsAsync(string referrerId, string emailHash)
        {
            var referralsByEmail = await _referralHotelsRepository.GetByEmailHashAsync(emailHash, null, null);

            return referralsByEmail.Any(x => x.ReferrerId == referrerId);
        }

        private async Task<bool> LimitExceededAsync(string customerId)
        {
            var referrals = await _referralHotelsRepository.GetByReferrerIdAsync(customerId);

            var lastReferrals = referrals.Where(x => x.CreationDateTime + _referralLimitPeriod > DateTime.UtcNow);

            return lastReferrals.Count() >= _referralLimit;
        }

        private string GenerateConfirmationToken()
        {
            return _hashingManager
                .GenerateBase(Guid.NewGuid().ToString())
                .Substring(
                    0,
                    _settingsService.GetLeadConfirmationTokenLength());
        }

        private static bool ReferralExpired(ReferralHotelEncrypted referral)
        {
            return referral.ExpirationDateTime < DateTime.UtcNow;
        }
        
        //note: don't change, required for backwards compatibility with the contract
        private static string[] SplitFullNameIntoNameAndSurname(string fullName)
        {
            string[] nameSurname = new string[2];
            string[] nameSurnameTemp = fullName.Trim(' ').Split(' ');
            if(nameSurnameTemp.Length == 1) {
                nameSurname[0] = fullName;
                nameSurname[1] = string.Empty;
            } else
                for (int i = 0; i < nameSurnameTemp.Length; i++)
                {
                    if (i < nameSurnameTemp.Length - 1)
                    {
                        if (!string.IsNullOrEmpty(nameSurname[0]))
                            nameSurname[0] += " " + nameSurnameTemp[i];
                        else
                            nameSurname[0] += nameSurnameTemp[i];
                    }
                    else
                        nameSurname[1] = nameSurnameTemp[i];
                }
            return nameSurname;
        }
    }
}
