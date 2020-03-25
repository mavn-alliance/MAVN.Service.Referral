using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Service.Referral.MsSqlRepositories;
using Lykke.Service.Referral.MsSqlRepositories.Repositories;
using Lykke.Service.Referral.Tests.MsSqlRepositories.Fixtures;
using Xunit;

namespace Lykke.Service.Referral.Tests.MsSqlRepositories
{
    public class PropertyPurchaseRepositoryTests : IClassFixture<PropertyPurchaseRepositoryFixture>
    {
        private readonly PropertyPurchaseRepository _propertyPurchaseRepository;

        public PropertyPurchaseRepositoryTests()
        {
            var contextFixture = new PropertyPurchaseRepositoryFixture();

            var bonusEngineContext = contextFixture.ReferralContext;

            var msSqlContextFactory = new SqlContextFactoryFake<ReferralContext>(c => bonusEngineContext);

            _propertyPurchaseRepository = new PropertyPurchaseRepository(
                msSqlContextFactory, MapperHelper.CreateAutoMapper());
        }

        [Fact]
        public async Task GetsAll()
        {
            //Act
            var allEntities = await _propertyPurchaseRepository.GetAsync();
            
            //Assert
            Assert.Equal(3, allEntities.Count);
        }
    }
}
