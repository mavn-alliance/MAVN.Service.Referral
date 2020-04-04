using System.Threading.Tasks;
using Falcon.Common;
using Lykke.Service.NotificationSystem.SubscriberContract;
using Lykke.Service.UrlShortener.Client.Models;
using Moq;
using Xunit;

namespace MAVN.Service.Referral.Tests.DomainServices.Services
{
    public class NotificationPublisherServiceTests
    {
        [Fact]
        public async Task ShouldSendEmailWithCorrectFirstNameAndLastName_WhenValidDataIsPassed()
        {
            // Arrange
            var fixture = new NotificationPublisherServiceTestsFixture();
            const string confirmToken = "token";
            var phone = PhoneUtils.GetE164FormattedNumber(fixture.PhoneNumber, fixture.CountryCode);

            fixture.ShortenUrlResponseModel = new ShortenUrlResponseModel
            {
                ShortenedUrl = "short token" + confirmToken
            };

            // Act
            await fixture.Service.LeadConfirmRequestAsync(
                fixture.CustomerId.ToString(), 
                phone, 
                confirmToken);

            // Assert
            fixture.SmsEventMock.Verify(c =>
                c.PublishAsync(It.Is<SmsEvent>(x =>
                    x.TemplateParameters.ContainsKey("target_phonenumber") && x.TemplateParameters.ContainsValue(phone) &&
                    x.TemplateParameters.ContainsKey("AcceptLink") && x.TemplateParameters.ContainsValue(fixture.ShortenUrlResponseModel.ShortenedUrl) &&
                    x.CustomerId == fixture.CustomerId.ToString())));

            //string.Format(_leadConfirmEmailLinkFormat, confirmToken)
        }
    }
}
