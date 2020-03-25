using Lykke.Service.Referral.MsSqlRepositories;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Service.Referral.Tests.MsSqlRepositories.Fixtures
{
    public class BaseRepositoryFixture
    {
        public ReferralContext ReferralContext => GetInMemoryContextWithSeededData();

        private ReferralContext GetInMemoryContextWithSeededData()
        {
            var context = CreateDataContext();
            
            Seed(context);
            
            return context;
        }

        protected virtual void Seed(ReferralContext context)
        {
            
        }

        private ReferralContext CreateDataContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(nameof(ReferralContext))
                .Options;

            var context = new ReferralContext("referral", options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            return context;
        }

        public void Dispose()
        {
            ReferralContext?.Dispose();
        }
    }
}