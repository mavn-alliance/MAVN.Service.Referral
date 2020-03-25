using System;
using System.Collections.Generic;
using Lykke.Service.Referral.MsSqlRepositories;
using Lykke.Service.Referral.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Service.Referral.Tests.MsSqlRepositories.Fixtures
{
    public class ReferralRepositoryFixture : BaseRepositoryFixture
    {
        protected override void Seed(ReferralContext context)
        {
            context.Referrals.AddRange(new List<ReferralEntity>
            {
                new ReferralEntity
                {
                    CustomerId = Guid.NewGuid()
                },
                new ReferralEntity
                {
                    CustomerId = Guid.Parse("57e80137-984c-44f0-ad6f-b555d46cd934"),
                    ReferralCode = "400484e213f14bc6"
                },
                new ReferralEntity
                {
                    CustomerId = Guid.NewGuid()
                }
            });
            context.SaveChanges();
        }
    }
}
