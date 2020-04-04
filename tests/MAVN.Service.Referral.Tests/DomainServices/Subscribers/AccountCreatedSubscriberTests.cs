using System;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.Service.CustomerManagement.Contract.Events;
using MAVN.Service.Referral.Domain.Services;
using MAVN.Service.Referral.DomainServices.Subscribers;
using Moq;
using Xunit;

namespace MAVN.Service.Referral.Tests.DomainServices.Subscribers
{
    public class AccountCreatedSubscriberTests
    {
        [Fact]
        public async Task ShouldNotProcessMessageAsync_WhenInvalidIdentifierIsPassed()
        {
            var friendReferralServiceMock = new Mock<IFriendReferralService>();

            var subscriber = new AccountCreatedSubscriber(
                "test",
                "test",
                friendReferralServiceMock.Object,
                EmptyLogFactory.Instance);

            await subscriber.StartProcessingAsync(new CustomerRegistrationEvent()
            {
                CustomerId = "not valid",
                ReferralCode = "ref"
            });

            friendReferralServiceMock.Verify(
                c => c.CreateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ShouldProcessMessageAsync_WhenValidIdentifierIsPassed()
        {
            var friendReferralServiceMock = new Mock<IFriendReferralService>();

            var subscriber = new AccountCreatedSubscriber(
                "test",
                "test",
                friendReferralServiceMock.Object,
                EmptyLogFactory.Instance);

            await subscriber.StartProcessingAsync(new CustomerRegistrationEvent()
            {
                CustomerId = Guid.NewGuid().ToString("D"),
                ReferralCode = "ref"
            });

            friendReferralServiceMock.Verify(
                c => c.ConfirmAsync(It.IsAny<string>(), It.IsAny<Guid>()),
                Times.Once);
        }

        [Fact]
        public async Task ShouldNotProcessMessageAsync_WhenNoReferralCodeIsPassed()
        {
            var friendReferralServiceMock = new Mock<IFriendReferralService>();

            var subscriber = new AccountCreatedSubscriber(
                "test",
                "test",
                friendReferralServiceMock.Object,
                EmptyLogFactory.Instance);

            await subscriber.StartProcessingAsync(new CustomerRegistrationEvent()
            {
                CustomerId = Guid.NewGuid().ToString("D")
            });

            friendReferralServiceMock.Verify(
                c => c.CreateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}
