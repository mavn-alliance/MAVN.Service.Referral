using System;
using System.Threading.Tasks;
using Lykke.Service.Referral.Domain.Exceptions;
using Lykke.Service.Referral.Domain.Models;
using Moq;
using Xunit;

namespace Lykke.Service.Referral.Tests.DomainServices.Services
{
    public class PropertyPurchaseServiceTests
    {
        [Fact]
        public async Task ShouldPublishPropertyPurchaseEvent_WhenValidDataIsPassed()
        {
            // Arrange
            var fixture = new PropertyPurchaseServiceTestsFixture()
            {
                PropertyPurchase = null
            };

            // Act
            var dateTime = DateTime.UtcNow;
            var referralLeadId = Guid.NewGuid();
            await fixture.Service.AddRealEstatePurchase(new PropertyPurchase
            {
                ReferralLeadId = referralLeadId,
                Timestamp = dateTime,
                CommissionNumber = 1
            });
        }

        [Fact]
        public async Task ShouldNotPublishPropertyPurchaseEvent_WhenReferralLeadDoesNotExists()
        {
            // Arrange
            var fixture = new PropertyPurchaseServiceTestsFixture()
            {
                ReferralLead = null
            };

            // Act
            var dateTime = DateTime.UtcNow;

            // Assert
            await Assert.ThrowsAsync<ReferralDoesNotExistException>(async () =>
            {
                await fixture.Service.AddRealEstatePurchase(new PropertyPurchase
                {
                    ReferralLeadId = Guid.NewGuid(),
                    Timestamp = dateTime,
                    CommissionNumber = 2
                });
            });

            fixture.PropertyPurchaseRepositorMock.Verify(x =>
                x.InsertAsync(It.IsAny<PropertyPurchase>()), Times.Never);
        }

        [Fact]
        public async Task ShouldGetAllPropertyPurchases_WhenMethodIsCalled()
        {
            // Arrange
            var fixture = new PropertyPurchaseServiceTestsFixture();

            // Act
            var result = await fixture.Service.GetPropertyPurchasesAsync();

            // Assert
            Assert.Equal(fixture.PropertyPurchases, result);
        }
    }
}
