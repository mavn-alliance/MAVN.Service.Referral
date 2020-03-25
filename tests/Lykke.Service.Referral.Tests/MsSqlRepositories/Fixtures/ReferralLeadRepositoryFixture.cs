using System;
using System.Collections.Generic;
using Lykke.Service.Referral.MsSqlRepositories;
using Lykke.Service.Referral.MsSqlRepositories.Entities;

namespace Lykke.Service.Referral.Tests.MsSqlRepositories.Fixtures
{
    public class ReferralLeadRepositoryFixture : BaseRepositoryFixture
    {
        protected override void Seed(ReferralContext context)
        {
            context.ReferralLeads.AddRange(
                new List<ReferralLeadEntity>
                {
                    new ReferralLeadEntity
                    {
                        Id = Guid.NewGuid(),
                        AgentId = Guid.Parse("78ceb436-29a9-499c-92fc-ec77152e32d8"),
                        State = ReferralLeadState.Pending
                    },
                    new ReferralLeadEntity
                    {
                        Id = Guid.Parse("57e80137-984c-44f0-ad6f-b555d46cd934"),
                        State = ReferralLeadState.Pending
                    },
                    new ReferralLeadEntity
                    {
                        Id = Guid.NewGuid(),
                        AgentId = Guid.Parse("78ceb436-29a9-499c-92fc-ec77152e32d8"),
                        EmailHash = "email@mail.com",
                        ConfirmationToken = "3l2k3h4lk",
                        State = ReferralLeadState.Pending
                    },
                    new ReferralLeadEntity
                    {
                        Id = Guid.NewGuid(),
                        AgentId = Guid.NewGuid(),
                        State = ReferralLeadState.Confirmed,
                        EmailHash = "email@mail.com",
                        PhoneCountryCodeId = 354,
                        PhoneNumberHash = "0881212838",
                        ConfirmationToken = "9l2l7a4lk"
                    },
                    new ReferralLeadEntity
                    {
                        Id = Guid.NewGuid(),
                        AgentId = Guid.NewGuid(),
                        State = ReferralLeadState.Confirmed,
                        EmailHash = "email@mail.com",
                        PhoneCountryCodeId = 359,
                        PhoneNumberHash = "0881212838",
                        ConfirmationToken = "9l2l7a4lk"
                    },
                    new ReferralLeadEntity
                    {
                        Id = Guid.NewGuid(),
                        AgentId = Guid.NewGuid(),
                        State = ReferralLeadState.Approved,
                        EmailHash = "another@yahoo.com",
                        PhoneCountryCodeId = 359,
                        PhoneNumberHash = "0884543421",
                        ConfirmationToken = "7a00a3a8p"
                    },
                    new ReferralLeadEntity
                    {
                        Id = Guid.NewGuid(),
                        AgentId = Guid.NewGuid(),
                        State = ReferralLeadState.Approved,
                        EmailHash = "another-one@yahoo.com",
                        PhoneCountryCodeId = 359,
                        PhoneNumberHash = "0884543421",
                        ConfirmationToken = "1b9yhklj3"
                    }
                });
            context.SaveChanges();
        }
    }
}
