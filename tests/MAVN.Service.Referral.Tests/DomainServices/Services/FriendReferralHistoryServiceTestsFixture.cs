using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.Campaign.Client;
using MAVN.Service.Campaign.Client.Models.Campaign.Responses;
using MAVN.Service.Campaign.Client.Models.Enums;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.CustomerProfile.Client.Models.Enums;
using MAVN.Service.CustomerProfile.Client.Models.Requests;
using MAVN.Service.CustomerProfile.Client.Models.Responses;
using MAVN.Service.Referral.Contract.Events;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.Domain.Services;
using MAVN.Service.Referral.DomainServices;
using MAVN.Service.Referral.DomainServices.Services;
using Moq;

namespace MAVN.Service.Referral.Tests.DomainServices.Services
{
    public class FriendReferralHistoryServiceTestsFixture
    {
        public FriendReferralHistoryServiceTestsFixture()
        {

            ReferralServiceMock = new Mock<IReferralService>(MockBehavior.Strict);
            FriendReferralPublisherMock = new Mock<IRabbitPublisher<FriendReferralEvent>>(MockBehavior.Strict);
            FriendReferralHistoryRepositoryMock = new Mock<IFriendReferralHistoryRepository>(MockBehavior.Strict);
            CustomerProfileClientMock = new Mock<ICustomerProfileClient>(MockBehavior.Strict);
            NotificationPublisherServiceMock = new Mock<INotificationPublisherService>(MockBehavior.Strict);
            CampaignClientMock = new Mock<ICampaignClient>(MockBehavior.Strict);

            Service = new FriendReferralService(
                ReferralServiceMock.Object,
                FriendReferralPublisherMock.Object,
                CustomerProfileClientMock.Object,
                FriendReferralHistoryRepositoryMock.Object,
                NotificationPublisherServiceMock.Object,
                CampaignClientMock.Object,
                EmptyLogFactory.Instance);

            Referral = new Domain.Models.Referral()
            {
                CustomerId = CustomerId,
                Id = Guid.NewGuid().ToString("D"),
                ReferralCode = ReferralCode
            };

            SetupCalls();
        }

        public Mock<ICampaignClient> CampaignClientMock { get; set; }
        public Mock<INotificationPublisherService> NotificationPublisherServiceMock { get; set; }
        public Mock<ICustomerProfileClient> CustomerProfileClientMock { get; set; }
        public Mock<IReferralService> ReferralServiceMock;
        public Mock<IRabbitPublisher<FriendReferralEvent>> FriendReferralPublisherMock;
        public Mock<IFriendReferralHistoryRepository> FriendReferralHistoryRepositoryMock;
        public FriendReferralService Service;

        public Guid CustomerId = Guid.NewGuid();
        public string ReferralCode = "ref123";
        public Domain.Models.Referral Referral;
        public List<Domain.Models.Referral> Referrals = new List<Domain.Models.Referral>();
        public string Email { get; set; } = "string@mail.com";
        public string FullName { get; set; } = "FName LName";
        public Guid CampaignId { get; set; } = Guid.NewGuid();

        public void SetupCalls()
        {
            ReferralServiceMock.Setup(c => c.GetReferralByReferralCodeAsync(ReferralCode))
                .ReturnsAsync(() => Referral);

            ReferralServiceMock.Setup(c => c.CreateReferralForCustomerIfNotExistAsync(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            FriendReferralHistoryRepositoryMock.Setup(c => 
                    c.CreateAsync(It.IsAny<ReferralFriend>()))
                .ReturnsAsync(() => new ReferralFriend());

            FriendReferralPublisherMock.Setup(c => c.PublishAsync(It.IsAny<FriendReferralEvent>()))
                .Returns(Task.CompletedTask);

            CustomerProfileClientMock
                .Setup(c => c.CustomerProfiles.GetByCustomerIdAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(() => new CustomerProfileResponse
                {
                    ErrorCode = CustomerProfileErrorCodes.None,
                    Profile = new CustomerProfile.Client.Models.Responses.CustomerProfile
                    {
                        Email = "another@mail.com"
                    }
                });

            CampaignClientMock.Setup(c => c.Campaigns.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new CampaignDetailResponseModel
                {
                    ErrorCode = CampaignServiceErrorCodes.None
                });
            
            CustomerProfileClientMock
                .Setup(c => c.CustomerProfiles.GetByEmailAsync(It.IsAny<GetByEmailRequestModel>()))
                .ReturnsAsync(() => new CustomerProfileResponse
                {
                    ErrorCode = CustomerProfileErrorCodes.CustomerProfileDoesNotExist
                });

            CustomerProfileClientMock
                .Setup(c => c.ReferralFriendProfiles.GetByEmailAndReferrerAsync(It.IsAny<ReferralFriendByEmailAndReferrerProfileRequest>()))
                .ReturnsAsync(() => new ReferralFriendProfileResponse
                {
                    ErrorCode = ReferralFriendProfileErrorCodes.ReferralFriendProfileDoesNotExist
                });

            CustomerProfileClientMock
                .Setup(c => c.ReferralFriendProfiles.AddAsync(It.IsAny<ReferralFriendProfileRequest>()))
                .ReturnsAsync(() => new ReferralFriendProfileResponse
                {
                    ErrorCode = ReferralFriendProfileErrorCodes.None
                });

            NotificationPublisherServiceMock.Setup(c => c.FriendReferralConfirmRequestAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            ReferralServiceMock.Setup(c => c.GetOrCreateReferralForCustomerIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => "someCode");
        }
    }
}
