using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.Campaign.Client;
using Lykke.Service.Campaign.Client.Models.Enums;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.CustomerProfile.Client.Models.Enums;
using Lykke.Service.CustomerProfile.Client.Models.Requests;
using Lykke.Service.Referral.Contract.Events;
using Lykke.Service.Referral.Domain.Exceptions;
using Lykke.Service.Referral.Domain.Models;
using Lykke.Service.Referral.Domain.Repositories;
using Lykke.Service.Referral.Domain.Services;

namespace Lykke.Service.Referral.DomainServices.Services
{
    public class FriendReferralService: IFriendReferralService
    {
        private readonly IReferralService _referralService;
        private readonly IRabbitPublisher<FriendReferralEvent> _friendReferralPublisher;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IFriendReferralHistoryRepository _friendReferralHistoryRepository;
        private readonly INotificationPublisherService _notificationPublisherService;
        private readonly ICampaignClient _campaignClient;
        private readonly ILog _log;

        public FriendReferralService(
            IReferralService referralService,
            IRabbitPublisher<FriendReferralEvent> friendReferralPublisher, 
            ICustomerProfileClient customerProfileClient,
            IFriendReferralHistoryRepository friendReferralHistoryRepository,
            INotificationPublisherService notificationPublisherService,
            ICampaignClient campaignClient,
            ILogFactory logFactory)
        {
            _referralService = referralService;
            _friendReferralPublisher = friendReferralPublisher;
            _customerProfileClient = customerProfileClient;
            _friendReferralHistoryRepository = friendReferralHistoryRepository;
            _notificationPublisherService = notificationPublisherService;
            _campaignClient = campaignClient;
            _log = logFactory.CreateLog(this);
        }

        public async Task<FriendReferralHistory> GetAllReferralsForCustomerAsync(Guid customerId)
        {
            return await _friendReferralHistoryRepository.GetAllReferralsForCustomerAsync(customerId);
        }

        public async Task<ReferralFriend> CreateAsync(Guid referrerId, Guid campaignId, string email, string fullName)
        {
            var profile = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(referrerId.ToString("D"));

            if (profile.ErrorCode == CustomerProfileErrorCodes.CustomerProfileDoesNotExist)
            {
                throw new CustomerDoesNotExistException($"Connector with id '{referrerId}' does not exist.");
            }

            if (string.Compare(profile.Profile.Email, email, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                throw new ReferYourselfException($"Cannot refer to yourself.");
            }

            var campaign = await _campaignClient.Campaigns.GetByIdAsync(campaignId.ToString("D"));

            if (campaign.ErrorCode == CampaignServiceErrorCodes.EntityNotFound)
            {
                throw new CampaignNotFoundException($"Campaign with id '{campaignId}' not found.");
            }

            var existingCustomer =
                await _customerProfileClient.CustomerProfiles.GetByEmailAsync(new GetByEmailRequestModel
                {
                    Email = email,
                    IncludeNotVerified = true
                });

            if (existingCustomer.ErrorCode == CustomerProfileErrorCodes.None)
            {
                throw new ReferralAlreadyConfirmedException($"Customer with such email already exists.");
            }

            var existingFriendProfile =
                await _customerProfileClient.ReferralFriendProfiles.GetByEmailAndReferrerAsync(
                    new ReferralFriendByEmailAndReferrerProfileRequest
                    {
                        Email = email,
                        ReferrerId = referrerId
                    });

            if (existingFriendProfile.ErrorCode == ReferralFriendProfileErrorCodes.None)
            {
                throw new ReferralAlreadyExistException($"Referral with such email for this referrer '{referrerId}' already exists.");
            }

            var referralCode = await _referralService.GetOrCreateReferralForCustomerIdAsync(referrerId);

            var referralFriend = new ReferralFriend
            {
                Email = email,
                FullName = fullName,
                ReferrerId = referrerId,
                CampaignId = campaignId,
                CreationDateTime = DateTime.UtcNow,
                State = ReferralFriendState.Pending
            };

            referralFriend = await _friendReferralHistoryRepository.CreateAsync(referralFriend);

            var response = await _customerProfileClient.ReferralFriendProfiles.AddAsync(new ReferralFriendProfileRequest
            {
                Email = email,
                FullName = fullName,
                ReferralFriendId = referralFriend.Id,
                ReferrerId = referrerId
            });

            if (response.ErrorCode != ReferralFriendProfileErrorCodes.None)
            {
                _log.Error(message: "An error occurred while creating referral friend profile",
                    context: $"referralFriendId: {referralFriend.Id}");
            }

            await _notificationPublisherService.FriendReferralConfirmRequestAsync(
                referrerId.ToString("D"),
                referralCode,
                profile.Profile.FirstName,
                profile.Profile.LastName,
                email);

            _log.Info("Referral friend created", context: $"referralFriendId: {referralFriend.Id}");

            return referralFriend;
        }

        public async Task<(bool isSuccessful, string errorMessage)> ConfirmAsync(string referralCode, Guid referredId)
        {
            Domain.Models.Referral referrer;

            try
            {
                referrer = await _referralService.GetReferralByReferralCodeAsync(referralCode);
            }
            catch (CustomerNotFoundException e)
            {
                _log.Warning(message: e.Message, exception: e);
                return (true, string.Empty);
            }

            var referred = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(referredId.ToString("D"), true);

            if (referred.ErrorCode == CustomerProfileErrorCodes.CustomerProfileDoesNotExist)
            {
                return (false, $"Connector with id '{referredId}' does not exist.");
            }

            var referredFiendProfile =
                await _customerProfileClient.ReferralFriendProfiles.GetByEmailAndReferrerAsync(new ReferralFriendByEmailAndReferrerProfileRequest
                {
                    Email = referred.Profile.Email,
                    ReferrerId = referrer.CustomerId
                });

            var friendReferral =
                await _friendReferralHistoryRepository.GetByIdAsync(referredFiendProfile.Data.ReferralFriendId);
            
            if (friendReferral == null)
            {
                return (false, $"Referral not found for referred '{referredId}' and referrer '{referrer.CustomerId}'");
            }

            // Generating a referral code for the new referral
            await _referralService.CreateReferralForCustomerIfNotExistAsync(referredId);

            friendReferral.State = ReferralFriendState.Accepted;
            friendReferral.ReferredId = referredId;

            await _friendReferralHistoryRepository.UpdateAsync(friendReferral);

            _log.Info("Referral friend confirmed", context: $"referralFriendId: {friendReferral.Id}");

            return (true, string.Empty);
        }

        public async Task<(bool isSuccessful, string errorMessage)> AcceptAsync(Guid customerId)
        {
            var friendReferral = await _friendReferralHistoryRepository.GetAcceptedAsync(customerId);

            if (friendReferral == null)
            {
                _log.Info($"No accepted friend referral for customer '{customerId}'");

                return (true, string.Empty);
            }

            friendReferral.State = ReferralFriendState.Confirmed;
            await _friendReferralHistoryRepository.UpdateAsync(friendReferral);

            await _friendReferralPublisher.PublishAsync(new FriendReferralEvent
            {
                ReferredId = friendReferral.ReferredId.ToString("D"),
                ReferrerId = friendReferral.ReferrerId.ToString("D"),
                ReferralId = friendReferral.Id.ToString("D"),
                Timestamp = DateTime.UtcNow
            });

            _log.Info("Referral friend accepted", context: $"referralFriendId: {friendReferral.Id}");

            return (true, string.Empty);
        }

        public async Task<IReadOnlyList<ReferralFriend>> GetByReferrerIdAsync(
            Guid referrerId,
            Guid? campaignId, 
            IEnumerable<ReferralFriendState> states)
        {
            var referralFriends = await _friendReferralHistoryRepository.GetByReferrerIdAsync(referrerId, campaignId, states);

            var result = await PopulateSensitiveDataAsync(referralFriends);

            return result;
        }

        public async Task<IReadOnlyList<ReferralFriend>> GetReferralFriendsByReferralIdsAsync(List<Guid> referralIds)
        {
            var referralFriends = await _friendReferralHistoryRepository.GetReferralFriendsByReferralIdsAsync(referralIds);

            var result = await PopulateSensitiveDataAsync(referralFriends);

            return result;
        }

        private async Task<List<ReferralFriend>> PopulateSensitiveDataAsync(IReadOnlyList<ReferralFriend> referralFriends)
        {
            var result = new List<ReferralFriend>();

            foreach (var referralFriend in referralFriends)
            {
                var friendProfileResponse = await _customerProfileClient.ReferralFriendProfiles.GetByIdAsync(referralFriend.Id);

                if (friendProfileResponse.ErrorCode != ReferralFriendProfileErrorCodes.None)
                {
                    _log.Warning(message: "Friend profile was not found for friend referral profile.", context: referralFriend);
                    continue;
                }

                referralFriend.FullName = friendProfileResponse.Data.FullName;
                referralFriend.Email = friendProfileResponse.Data.Email;

                result.Add(referralFriend);
            }

            return result;
        }
    }
}
