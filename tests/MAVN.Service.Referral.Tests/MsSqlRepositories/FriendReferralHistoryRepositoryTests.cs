using System;
using System.Threading.Tasks;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.Referral.MsSqlRepositories;
using MAVN.Service.Referral.MsSqlRepositories.Repositories;
using MAVN.Service.Referral.Tests.MsSqlRepositories.Fixtures;
using Xunit;

namespace MAVN.Service.Referral.Tests.MsSqlRepositories
{
    public class FriendReferralHistoryRepositoryTests : IClassFixture<FriendReferralHistoryRepositoryFixture>
    {
        private readonly FriendReferralHistoryRepository _friendReferralHistoryRepository;

        public FriendReferralHistoryRepositoryTests()
        {
            var contextFixture = new FriendReferralHistoryRepositoryFixture();

            var bonusEngineContext = contextFixture.ReferralContext;

            var msSqlContextFactory = new SqlContextFactoryFake<ReferralContext>(c => bonusEngineContext);

            _friendReferralHistoryRepository = new FriendReferralHistoryRepository(msSqlContextFactory, MapperHelper.CreateAutoMapper());
        }

        [Theory]
        [InlineData("57e80137-984c-44f0-ad6f-b555d46cd934", 2)]
        [InlineData("1fe0bd6d-af12-4ff0-a64c-c796b21e3b8e", 0)]
        public async Task FetchAllReferralsForClientIfAny(string customerId, int referredCount)
        {
            //Arrange
            var customerIdGuid = Guid.Parse(customerId);

            //Act
            var referrals = await _friendReferralHistoryRepository.GetAllReferralsForCustomerAsync(customerIdGuid);

            //Assert
            Assert.Equal(customerId, referrals.ReferrerId);
            Assert.Equal(referredCount, referrals.ReferredIds.Count);
        }
    }
}
