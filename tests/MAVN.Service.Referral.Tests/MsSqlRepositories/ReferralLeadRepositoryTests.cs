using System;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using MAVN.Service.Referral.MsSqlRepositories;
using MAVN.Service.Referral.MsSqlRepositories.Entities;
using MAVN.Service.Referral.MsSqlRepositories.Repositories;
using MAVN.Service.Referral.Tests.MsSqlRepositories.Fixtures;
using Xunit;

namespace MAVN.Service.Referral.Tests.MsSqlRepositories
{
    public class ReferralLeadRepositoryTests : IClassFixture<ReferralLeadRepositoryFixture>
    {
        private readonly ReferralLeadRepository _referralLeadRepository;
        
        public ReferralLeadRepositoryTests()
        {
            var contextFixture = new ReferralLeadRepositoryFixture();

            var bonusEngineContext = contextFixture.ReferralContext;

            var msSqlContextFactory = new SqlContextFactoryFake<ReferralContext>(c => bonusEngineContext);

            _referralLeadRepository = new ReferralLeadRepository(msSqlContextFactory, MapperHelper.CreateAutoMapper());
        }

        [Fact]
        public async Task GetsByReferId()
        {
            //Arrange
            var referIdGuid = Guid.Parse("57e80137-984c-44f0-ad6f-b555d46cd934");
            
            //Act
            var entity = await _referralLeadRepository.GetAsync(referIdGuid);
            
            //Assert
            Assert.Equal(referIdGuid, entity.Id);
        }

        [Fact]
        public async Task GetsAll()
        {
            //Arrange
            var agentId = Guid.Parse("78ceb436-29a9-499c-92fc-ec77152e32d8");

            //Act
            var allEntities = await _referralLeadRepository.GetForReferrerAsync(agentId, null, null);
            
            //Assert
            Assert.Equal(2, allEntities.Count);
        }

        [Theory]
        [InlineData("57e80137-984c-44f0-ad6f-b555d46cd934", true)]
        [InlineData("400484e2-13f1-4bc6-b079-66b25d1b71f6", false)]
        public async Task ChecksIfExists(string referId, bool shouldExist)
        {
            //Arrange
            var referIdGuid = Guid.Parse(referId);
            
            //Act
            var exists = await _referralLeadRepository.DoesExistAsync(referIdGuid);
            
            //Assert
            Assert.Equal(shouldExist, exists);
        }
        
        [Theory]
        [InlineData("email@mail.com", 3)]
        [InlineData("another@yahoo.com", 1)]
        [InlineData("notexisting@email.com", 0)]
        public async Task GetsByEmail(string email, int count)
        {
            //Act
            var entries = await _referralLeadRepository.GetByEmailHashAsync(email);
            
            //Assert
            Assert.Equal(count, entries.Count);
            
            foreach (var entry in entries)
                Assert.Equal(email, entry.EmailHash);
        }
        
        [Theory]
        [InlineData(359, "0884543421", 2)]
        [InlineData(359, "0881212838", 1)]
        [InlineData(359, "0881215701", 0)]
        public async Task GetsByPhone(int countryCodeId, string phone, int count)
        {
            //Act
            var entries = await _referralLeadRepository.GetByPhoneNumberHashAsync(countryCodeId, phone);
            
            //Assert
            Assert.Equal(count, entries.Count);
            
            foreach (var entry in entries)
                Assert.Equal(phone, entry.PhoneNumberHash);
        }
        
        [Theory]
        [InlineData("3l2k3h4lk")]
        [InlineData("7a00a3a8p")]
        [InlineData("1b9yhklj3")]
        public async Task GetsByConfirmationToken(string confirmationToken)
        {
            //Act
            var entry = await _referralLeadRepository.GetByConfirmationTokenAsync(confirmationToken);
            
            //Assert
            Assert.Equal(confirmationToken, entry.ConfirmationToken);
        }

        [Fact]
        public async Task GetsApproved()
        {
            //Arrange
            var expectedCount = 2;
            
            //Act
            var entries = await _referralLeadRepository.GetApprovedAsync();
            
            //Assert
            Assert.Equal(expectedCount, entries.Count);
            
            foreach (var entry in entries)
                Assert.Equal(ReferralLeadState.Approved.ToString(), entry.State.ToString());
        }
    }
}
