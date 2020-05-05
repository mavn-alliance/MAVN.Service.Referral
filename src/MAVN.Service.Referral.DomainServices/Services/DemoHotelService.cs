using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.Campaign.Client;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.CustomerProfile.Client.Models.Enums;
using MAVN.Service.CustomerProfile.Client.Models.Requests;
using MAVN.Service.Referral.Contract.Events;
using MAVN.Service.Referral.Domain.Exceptions;
using MAVN.Service.Referral.Domain.Managers;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.Domain.Services;
using MAVN.Service.Referral.DomainServices.Extensions;

namespace MAVN.Service.Referral.DomainServices.Services
{
    public class DemoHotelService : IDemoHotelService
    {
        private const string ConditionType = "hotel-stay-referral";

        private readonly string _baseCurrencyCode;
        private readonly IReferralHotelsRepository _referralHotelsRepository;
        private readonly ISettingsService _settingsService;
        private readonly INotificationPublisherService _notificationPublisherService;
        private readonly IHashingManager _hashingManager;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly ICampaignClient _campaignClient;
        private readonly TimeSpan _referralExpirationPeriod;
        private readonly IMapper _mapper;
        private readonly IRabbitPublisher<HotelReferralUsedEvent> _hotelReferralUsedPublisher;
        private readonly ILog _log;

        public DemoHotelService(
            string baseCurrencyCode,
            IReferralHotelsRepository referralHotelsRepository,
            ISettingsService settingsService,
            INotificationPublisherService notificationPublisherService,
            IHashingManager hashingManager,
            ICustomerProfileClient customerProfileClient,
            ICampaignClient campaignClient,
            ILogFactory logFactory,
            TimeSpan referralExpirationPeriod,
            IMapper mapper,
            IRabbitPublisher<HotelReferralUsedEvent> hotelReferralUsedPublisher)
        {
            _baseCurrencyCode = baseCurrencyCode;
            _referralHotelsRepository = referralHotelsRepository;
            _settingsService = settingsService;
            _notificationPublisherService = notificationPublisherService;
            _hashingManager = hashingManager;
            _customerProfileClient = customerProfileClient;
            _campaignClient = campaignClient;
            _referralExpirationPeriod = referralExpirationPeriod;
            _mapper = mapper;
            _hotelReferralUsedPublisher = hotelReferralUsedPublisher;
            _log = logFactory.CreateLog(this);
        }

        public async Task<ReferralHotel> CreateHotelReferralAsync(
            Guid? campaignId,
            string email,
            string referrerId,
            string fullName,
            int phoneCountryCodeId,
            string phoneNumber)
        {
            ReferralHotel createdReferralHotel = null;
            try
            {
                if (!await CustomerExistsAsync(referrerId))
                {
                    throw new CustomerDoesNotExistException();
                }
            
                var emailHash = email.ToSha256Hash();

                var creationDateTime = DateTime.UtcNow;

                var referralHotelEncrypted = new ReferralHotelEncrypted
                {
                    EmailHash = emailHash,
                    ReferrerId = referrerId,
                    FullNameHash = fullName.ToSha256Hash(),
                    PhoneCountryCodeId = phoneCountryCodeId,
                    PhoneNumberHash = phoneNumber.ToSha256Hash(),
                    ConfirmationToken = GenerateConfirmationToken(),
                    CreationDateTime = creationDateTime,
                    ExpirationDateTime = creationDateTime + _referralExpirationPeriod,
                    State = ReferralHotelState.Pending,
                    CampaignId = campaignId,
                };
                if (campaignId != null)
                {
                    var campaign = await _campaignClient.Campaigns.GetByIdAsync(campaignId.Value.ToString());
                    var condition = campaign?.Conditions.FirstOrDefault(c => c.Type == ConditionType);
                    if (condition?.PartnerIds != null && condition.PartnerIds.Length > 0)
                        referralHotelEncrypted.PartnerId = condition.PartnerIds.First().ToString();
                }

                referralHotelEncrypted = await _referralHotelsRepository.CreateAsync(referralHotelEncrypted);

                createdReferralHotel = _mapper.Map<ReferralHotel>(referralHotelEncrypted);

                createdReferralHotel.Email = email;

                var response = await _customerProfileClient.ReferralHotelProfiles.AddAsync(new ReferralHotelProfileRequest
                {
                    ReferralHotelId = Guid.Parse(createdReferralHotel.Id),
                    Email = createdReferralHotel.Email,
                    Name = fullName,
                    PhoneNumber = phoneNumber,
                });

                if (response.ErrorCode != ReferralHotelProfileErrorCodes.None)
                {
                    _log.Error(message: "An error occurred while creating referral hotel profile",
                        context: $"referralHotelId: {createdReferralHotel.Id}");
                }

                await _notificationPublisherService.HotelReferralConfirmRequestAsync(createdReferralHotel.ReferrerId,
                    createdReferralHotel.Email, _referralExpirationPeriod, createdReferralHotel.ConfirmationToken);

                _log.Info("Referral hotel created", context: $"referralHotelId: {createdReferralHotel.Id}");
            }
            catch (Exception e)
            {
                _log.Error("Demo service failed to process the request.", e);
            }

            return createdReferralHotel;
        }

        public async Task<ReferralHotel> GetReferralHotelByConfirmationTokenAsync(string confirmationToken)
        {
            var hotelReferralEncrypted = await _referralHotelsRepository.GetByConfirmationTokenAsync(confirmationToken);

            return await DecryptAsync(hotelReferralEncrypted);
        }

        public async Task<ReferralHotel> ConfirmReferralHotelAsync(string confirmationToken)
        {
            ReferralHotel confirmReferralHotelAsync = null;
            try
            {
                var referralHotelEncrypted = await _referralHotelsRepository.GetByConfirmationTokenAsync(confirmationToken);

                if (referralHotelEncrypted == null)
                {
                    throw new ReferralDoesNotExistException();
                }

                if (referralHotelEncrypted.State == ReferralHotelState.Used)
                {
                    confirmReferralHotelAsync = await DecryptAsync(referralHotelEncrypted);
                    return confirmReferralHotelAsync;
                }

                referralHotelEncrypted.State = ReferralHotelState.Used;
                referralHotelEncrypted = await _referralHotelsRepository.UpdateAsync(referralHotelEncrypted);

                await PublishHotelUsedEvent(referralHotelEncrypted);

                confirmReferralHotelAsync = await DecryptAsync(referralHotelEncrypted);

                _log.Info($"PublishHotelUsedEvent '{confirmationToken}'");

                return confirmReferralHotelAsync;
            }
            catch (Exception e)
            {
                _log.Error("Demo service failed to process the request.", e);
            }

            return confirmReferralHotelAsync;
        }

        private async Task PublishHotelUsedEvent(ReferralHotelEncrypted referralHotelEncrypted)
        {
            await _hotelReferralUsedPublisher.PublishAsync(new HotelReferralUsedEvent
            {
                CustomerId = referralHotelEncrypted.ReferrerId,
                Amount = 1,
                CurrencyCode = _baseCurrencyCode,
                PartnerId = referralHotelEncrypted.PartnerId,
                ReferralId = referralHotelEncrypted.Id,
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

        private async Task<bool> CustomerExistsAsync(string customerId)
        {
            var response = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(customerId);

            return response.ErrorCode != CustomerProfileErrorCodes.CustomerProfileDoesNotExist;
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
            }

            return referralHotel;
        }
    }
}
