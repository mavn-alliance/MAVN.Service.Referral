using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.Referral.Domain.Exceptions;
using Moq;
using Xunit;

namespace MAVN.Service.Referral.Tests.DomainServices.Services
{
    public class ReferralServiceTests
    {
        [Fact]
        public async Task ShouldGetReferral_WhenValidDataIsPassed()
        {
            // Arrange
            var fixture = new ReferralServiceTestsFixture();

            // Act
            var result = await fixture.Service.GetReferralByCustomerIdAsync(fixture.CustomerId);

            // Assert
            Assert.Equal(fixture.Referral, result);
        }

        [Fact]
        public async Task ShouldThrowCustomerNotFound_WhenDataDoesNotExist()
        {
            // Arrange
            var fixture = new ReferralServiceTestsFixture()
            {
                Referral = null
            };

            // Act

            // Assert
            await Assert.ThrowsAsync<CustomerNotFoundException>(async () =>
            {
                await fixture.Service.GetReferralByCustomerIdAsync(fixture.CustomerId);
            });
        }

        [Fact]
        public async Task ShouldReturnExistingReferralCode_WhenCustomAlreadyHasOne()
        {
            // Arrange
            var fixture = new ReferralServiceTestsFixture();

            // Act
            var result = await fixture.Service.GetOrCreateReferralForCustomerIdAsync(fixture.CustomerId);

            // Assert
            Assert.Equal(fixture.ReferralCode, result);

            fixture.ReferralRepositoryMock.Verify(c => c.GetByReferralCodeAsync(It.IsAny<string>()), Times.Never);
            fixture.ReferralRepositoryMock.Verify(c => c.CreateIfNotExistAsync(It.IsAny<Domain.Models.Referral>()), Times.Never);
        }

        [Fact]
        public async Task ShouldGenerateReferral_WhenValidDataIsPassed()
        {
            // Arrange
            var fixture = new ReferralServiceTestsFixture()
            {
                Referral = null,
                Created = true
            };

            // Act
            var result = await fixture.Service.GetOrCreateReferralForCustomerIdAsync(fixture.CustomerId);

            // Assert
            Assert.Equal(fixture.ReferralCode, result);

            fixture.ReferralRepositoryMock.Verify(c => c.GetByReferralCodeAsync(It.IsAny<string>()), Times.Once);
            fixture.ReferralRepositoryMock.Verify(c => c.CreateIfNotExistAsync(It.IsAny<Domain.Models.Referral>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGenerateReferral_WhenCollisionForReferralCodeIsDetected()
        {
            // Arrange
            var referral = new Domain.Models.Referral
            {
                CustomerId = Guid.NewGuid(),
                Id = Guid.NewGuid().ToString("D"),
                ReferralCode = "123456"
            };

            var fixture = new ReferralServiceTestsFixture()
            {
                Referral = null,
                Created = true,
                Referrals = new List<Domain.Models.Referral>()
                {
                    referral,
                    referral
                }
            };

            // Act
            var result = await fixture.Service.GetOrCreateReferralForCustomerIdAsync(fixture.CustomerId);

            // Assert
            Assert.Equal(fixture.ReferralCode, result);

            fixture.ReferralRepositoryMock.Verify(c => c.GetByReferralCodeAsync(It.IsAny<string>()), Times.Exactly(3));
            fixture.ReferralRepositoryMock.Verify(c => c.CreateIfNotExistAsync(It.IsAny<Domain.Models.Referral>()), Times.Once);
        }

        [Fact]
        public async Task ShouldGetReferral_WhenExistingReferralCodeIsPassed()
        {
            // Arrange
            var referral = new Domain.Models.Referral
            {
                CustomerId = Guid.NewGuid(),
                Id = Guid.NewGuid().ToString("D"),
                ReferralCode = "123456"
            };

            var fixture = new ReferralServiceTestsFixture()
            {
                Referral = null,
                Referrals = new List<Domain.Models.Referral>()
                {
                    referral
                }
            };

            // Act
            var result = await fixture.Service.GetReferralByReferralCodeAsync(fixture.ReferralCode);

            // Assert
            Assert.Equal(referral, result);
        }

        [Fact]
        public async Task ShouldThrowCustomerNotFound_WhenNonExistingReferralCodeIsPassed()
        {
            // Arrange
            var referral = new Domain.Models.Referral
            {
                CustomerId = Guid.NewGuid(),
                Id = Guid.NewGuid().ToString("D"),
                ReferralCode = "123456"
            };

            var fixture = new ReferralServiceTestsFixture();

            // Act

            // Assert
            await Assert.ThrowsAsync<CustomerNotFoundException>(async () =>
            {
                await fixture.Service.GetReferralByReferralCodeAsync(fixture.ReferralCode);
            });
        }

        [Fact]
        public async Task ShouldThrowArgumentNullException_WhenNullReferralCodeIsPassed()
        {
            // Arrange
            var referral = new Domain.Models.Referral
            {
                CustomerId = Guid.NewGuid(),
                Id = Guid.NewGuid().ToString("D"),
                ReferralCode = "123456"
            };

            var fixture = new ReferralServiceTestsFixture();

            // Act

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await fixture.Service.GetReferralByReferralCodeAsync(null);
            });
        }

        [Fact]
        public async Task ShouldCreateCustomerReferral_WhenNonExistentCustomerIdIsPassed()
        {
            // Arrange
            var referral = new Domain.Models.Referral
            {
                CustomerId = Guid.NewGuid(),
                Id = Guid.NewGuid().ToString("D"),
                ReferralCode = "123456"
            };

            var fixture = new ReferralServiceTestsFixture()
            {
                Referral = null
            };

            // Act
            await fixture.Service.CreateReferralForCustomerIfNotExistAsync(fixture.CustomerId);

            // Assert
            fixture.ReferralRepositoryMock.Verify(c => 
                c.CreateIfNotExistAsync(It.Is<Domain.Models.Referral>(
                    x => x.CustomerId == fixture.CustomerId)), Times.Once);
        }

        [Fact]
        public async Task ShouldNotCreateCustomerReferral_WhenExistentCustomerIdIsPassed()
        {
            // Arrange
            var referral = new Domain.Models.Referral
            {
                CustomerId = Guid.NewGuid(),
                Id = Guid.NewGuid().ToString("D"),
                ReferralCode = "123456"
            };

            var fixture = new ReferralServiceTestsFixture()
            {
                Referral = referral
            };

            // Act
            await fixture.Service.CreateReferralForCustomerIfNotExistAsync(fixture.CustomerId);

            // Assert
            fixture.ReferralRepositoryMock.Verify(c =>
                c.CreateIfNotExistAsync(It.Is<Domain.Models.Referral>(
                    x => x.CustomerId == fixture.CustomerId)), Times.Never);
        }
    }
}
