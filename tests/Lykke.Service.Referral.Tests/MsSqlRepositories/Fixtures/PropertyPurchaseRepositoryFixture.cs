using System;
using System.Collections.Generic;
using Lykke.Service.Referral.MsSqlRepositories;
using Lykke.Service.Referral.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Service.Referral.Tests.MsSqlRepositories.Fixtures
{
    public class PropertyPurchaseRepositoryFixture : BaseRepositoryFixture
    {
        protected override void Seed(ReferralContext context)
        {
            context.PropertyPurchases.AddRange(
                new List<PropertyPurchaseEntity>()
                {
                    new PropertyPurchaseEntity
                    {
                        Id = Guid.NewGuid(),
                        ReferralLeadId = Guid.NewGuid()
                    },
                    new PropertyPurchaseEntity
                    {
                        Id = Guid.NewGuid(),
                        ReferralLeadId = Guid.Parse("57e80137-984c-44f0-ad6f-b555d46cd934")
                    },
                    new PropertyPurchaseEntity
                    {
                        Id = Guid.NewGuid(),
                        ReferralLeadId = Guid.NewGuid()
                    }
                });
            context.SaveChanges();
        }
    }
}
