using Xunit;

namespace Lykke.Service.Referral.Tests
{
    public class AutoMapperProfileTests
    {
        [Fact]
        public void Mapping_Configuration_Is_Correct()
        {
            // arrange
            var mapper = MapperHelper.CreateAutoMapper();

            // act
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            // assert
            Assert.True(true);
        }
    } 
}
