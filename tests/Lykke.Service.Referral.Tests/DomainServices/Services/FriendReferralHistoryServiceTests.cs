using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.CustomerProfile.Client.Models.Enums;
using Lykke.Service.CustomerProfile.Client.Models.Responses;
using Lykke.Service.Referral.Contract.Events;
using Lykke.Service.Referral.Domain.Exceptions;
using Lykke.Service.Referral.Domain.Models;
using Moq;
using Xunit;

namespace Lykke.Service.Referral.Tests.DomainServices.Services
{
    public class FriendReferralHistoryServiceTests
    {
        [Fact]
        public async Task ShouldPublishFriendReferralEvent_WhenValidDataIsPassed()
        {
            // Arrange
            var fixture = new FriendReferralHistoryServiceTestsFixture();
            var referredId = Guid.NewGuid();

            // Act;
            await fixture.Service.CreateAsync(fixture.CustomerId, fixture.CampaignId, fixture.Email, fixture.FullName);

            // Assert
            fixture.FriendReferralHistoryRepositoryMock.Verify(x =>
                x.CreateAsync(
                    It.Is<ReferralFriend>(c => c.ReferrerId == fixture.CustomerId)));
        }

        [Fact]
        public async Task ShouldNotPublishFriendReferralEvent_WhenNoCustomerIsFoundForReferralCode()
        {
            // Arrange
            var fixture = new FriendReferralHistoryServiceTestsFixture();

            fixture.CustomerProfileClientMock.Setup(c => c.CustomerProfiles.GetByCustomerIdAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(() => new CustomerProfileResponse
                {
                    ErrorCode = CustomerProfileErrorCodes.CustomerProfileDoesNotExist
                });

            var referredId = Guid.NewGuid();

            // Act;
            // Assert

            await Assert.ThrowsAsync<CustomerDoesNotExistException>(async () =>
                await fixture.Service.CreateAsync(referredId, fixture.CampaignId, fixture.Email, fixture.FullName));

        }

        [Fact]
        public async Task ShouldGetAllPurchaseReferrals_WhenCustomerIdIsPassed()
        {
            // Arrange
            var fixture = new FriendReferralHistoryServiceTestsFixture();

            var friendReferralHistory = new FriendReferralHistory
            {
                ReferrerId = Guid.NewGuid().ToString()
            };

            fixture.FriendReferralHistoryRepositoryMock.Setup(c => c.GetAllReferralsForCustomerAsync(It.IsAny<Guid>()))
                .ReturnsAsync(friendReferralHistory);


            // Act;
            var result = await fixture.Service.GetAllReferralsForCustomerAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(friendReferralHistory, result);
        }
    }
}
