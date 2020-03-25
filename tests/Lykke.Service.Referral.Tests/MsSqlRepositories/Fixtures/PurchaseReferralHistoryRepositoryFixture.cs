using System;
using System.Collections.Generic;
using Lykke.Service.Referral.MsSqlRepositories;
using Lykke.Service.Referral.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Service.Referral.Tests.MsSqlRepositories.Fixtures
{
    public class PurchaseReferralHistoryRepositoryFixture : BaseRepositoryFixture
    {
        protected override void Seed(ReferralContext context)
        {
            context.PurchaseReferrals.AddRange(
                new List<PurchaseReferralHistoryEntity>
                {
                    new PurchaseReferralHistoryEntity
                    {
                        Id = Guid.NewGuid(),
                        ReferrerId = Guid.NewGuid(),
                        ReferredId = Guid.NewGuid()
                    },
                    new PurchaseReferralHistoryEntity
                    {
                        Id = Guid.NewGuid(),
                        ReferrerId = Guid.Parse("57e80137-984c-44f0-ad6f-b555d46cd934"),
                        ReferredId = Guid.NewGuid()
                    },
                    new PurchaseReferralHistoryEntity
                    {
                        Id = Guid.NewGuid(),
                        ReferrerId = Guid.Parse("57e80137-984c-44f0-ad6f-b555d46cd934"),
                        ReferredId = Guid.NewGuid()
                    }
                });
            context.SaveChanges();
        }
    }
}