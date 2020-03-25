using System;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Service.Referral.MsSqlRepositories;
using Lykke.Service.Referral.MsSqlRepositories.Repositories;
using Lykke.Service.Referral.Tests.MsSqlRepositories.Fixtures;
using Xunit;

namespace Lykke.Service.Referral.Tests.MsSqlRepositories
{
    public class ReferralRepositoryTests : IClassFixture<ReferralRepositoryFixture>
    {
        private readonly ReferralRepository _referralRepository;

        public ReferralRepositoryTests()
        {
            var contextFixture = new ReferralRepositoryFixture();

            var bonusEngineContext = contextFixture.ReferralContext;

            var msSqlContextFactory = new SqlContextFactoryFake<ReferralContext>(c => bonusEngineContext);

            _referralRepository = new ReferralRepository(msSqlContextFactory, MapperHelper.CreateAutoMapper());
        }

        [Fact]
        public async Task GetsByCustomerId()
        {
            //Arrange
            var customerIdGuid = Guid.Parse("57e80137-984c-44f0-ad6f-b555d46cd934");

            //Act
            var entity = await _referralRepository.GetByCustomerIdAsync(customerIdGuid);

            //Assert
            Assert.Equal(customerIdGuid, entity.CustomerId);
        }

        [Fact]
        public async Task GetsByReferralCode()
        {
            //Arrange
            var referralCode = "400484e213f14bc6";

            //Act
            var entity = await _referralRepository.GetByReferralCodeAsync(referralCode);

            //Assert
            Assert.Equal(referralCode, entity.ReferralCode);
        }
    }
}
