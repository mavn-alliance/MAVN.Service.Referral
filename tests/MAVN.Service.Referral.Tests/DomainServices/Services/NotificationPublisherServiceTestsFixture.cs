using System;
using System.Threading.Tasks;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.CustomerProfile.Client.Models.Enums;
using Lykke.Service.CustomerProfile.Client.Models.Responses;
using Lykke.Service.NotificationSystem.SubscriberContract;
using MAVN.Service.Referral.Domain.Services;
using MAVN.Service.Referral.DomainServices;
using MAVN.Service.Referral.DomainServices.Services;
using Lykke.Service.UrlShortener.Client;
using Lykke.Service.UrlShortener.Client.Models;
using Moq;

namespace MAVN.Service.Referral.Tests.DomainServices.Services
{
    public class NotificationPublisherServiceTestsFixture
    {
        public NotificationPublisherServiceTestsFixture()
        {

            EmailMessageEventMock = new Mock<IRabbitPublisher<EmailMessageEvent>>(MockBehavior.Strict);
            SmsEventMock = new Mock<IRabbitPublisher<SmsEvent>>(MockBehavior.Strict);
            PushNotificationPublisher = new Mock<IRabbitPublisher<PushNotificationEvent>>(MockBehavior.Strict);
            CustomerProfileMock = new Mock<ICustomerProfileClient>(MockBehavior.Strict);
            UrlShortenerClientMock = new Mock<IUrlShortenerClient>(MockBehavior.Strict);

            EmailLinkFormat = "Link-{0}";

            Service = new NotificationPublisherService(
                EmailMessageEventMock.Object,
                SmsEventMock.Object,
                PushNotificationPublisher.Object,
                CustomerProfileMock.Object,
                UrlShortenerClientMock.Object,
                EmailLinkFormat, 
                string.Empty,
                string.Empty,
                EmailLinkFormat, 
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                true
                );

            FirstName = "firstn";
            LastName = "lastn";
            PhoneNumber = "123123123";
            CountryCode = "358";
            Email = "email";
            IsEmailVerified = true;

            CustomerProfileResponse = new CustomerProfileResponse
            {
                ErrorCode = CustomerProfileErrorCodes.None,
                Profile = new CustomerProfile
                {
                    CustomerId = CustomerId.ToString(),
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    IsEmailVerified = IsEmailVerified,
                    PhoneNumber = PhoneNumber,
                    Registered = DateTime.UtcNow
                }
            };
            
            ReferralHotelProfileResponse = new ReferralHotelProfileResponse
            {
                ErrorCode = ReferralHotelProfileErrorCodes.None,
                Data = new ReferralHotelProfile
                {
                    ReferralHotelId = Guid.Empty,
                    Email = Email
                }
            };
            ShortenUrlResponseModel = new ShortenUrlResponseModel {ShortenedUrl = EmailLinkFormat};

            SetupCalls();
        }

        public string CountryCode { get; set; }

        public Mock<IUrlShortenerClient> UrlShortenerClientMock { get; set; }

        public Mock<IRabbitPublisher<SmsEvent>> SmsEventMock { get; set; }

        public Mock<IRabbitPublisher<EmailMessageEvent>> EmailMessageEventMock { get; set; }

        public Mock<IRabbitPublisher<PushNotificationEvent>> PushNotificationPublisher { get; set; }

        public Mock<ICustomerProfileClient> CustomerProfileMock { get; set; }
        
        public string EmailLinkFormat { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public NotificationPublisherService Service;
        public CustomerProfileResponse CustomerProfileResponse;
        public ReferralHotelProfileResponse ReferralHotelProfileResponse;

        public Guid CustomerId = Guid.NewGuid();
        public ShortenUrlResponseModel ShortenUrlResponseModel;

        public void SetupCalls()
        {
            CustomerProfileMock.Setup(c => c.CustomerProfiles.GetByCustomerIdAsync(CustomerId.ToString(), false, false))
                .ReturnsAsync(() => CustomerProfileResponse);
            
            CustomerProfileMock.Setup(c => c.ReferralHotelProfiles.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid referralHotelId) => ReferralHotelProfileResponse);

            SmsEventMock.Setup(c => c.PublishAsync(It.IsAny<SmsEvent>()))
                .Returns(Task.CompletedTask);

            UrlShortenerClientMock.Setup(c => c.UrlShorteningApi.ShortenUrlAsync(It.IsAny<ShortenUrlRequestModel>()))
                .ReturnsAsync(() => ShortenUrlResponseModel);
        }
    }
}
