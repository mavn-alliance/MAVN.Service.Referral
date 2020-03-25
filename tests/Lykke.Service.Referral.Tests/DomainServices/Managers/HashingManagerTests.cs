using System;
using Lykke.Logs;
using Lykke.Service.Referral.DomainServices.Managers;
using Xunit;

namespace Lykke.Service.Referral.Tests.DomainServices.Managers
{
    public class HashingManagerTests
    {
        [Fact]
        public void ShouldGenerateBase36_WhenValidDataIsPassed()
        {
            // Arrange
            var input = "test1";

            var service = new HashingManager(EmptyLogFactory.Instance);
            // Act
            var result = service.GenerateBase(input);

            // Assert
            Assert.Equal("9e2dxudr6e6d3vm92jwibeq5gdk6awlh1v1luhgmb4m3h3mk8r", result);
        }

        [Fact]
        public void ShouldThrowArgumentNullException_WhenInvalidDataIsPassed()
        {
            // Arrange
            string input = null;

            var service = new HashingManager(EmptyLogFactory.Instance);
            // Act

            // Assert
                Assert.Throws<ArgumentNullException>(() =>
            {
                service.GenerateBase(input);
            });
        }
    }
}
