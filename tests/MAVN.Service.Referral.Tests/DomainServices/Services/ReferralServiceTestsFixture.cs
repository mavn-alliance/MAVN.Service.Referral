using System;
using System.Collections.Generic;
using Lykke.Logs;
using MAVN.Service.Referral.Domain.Managers;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.DomainServices;
using MAVN.Service.Referral.DomainServices.Services;
using Moq;

namespace MAVN.Service.Referral.Tests.DomainServices.Services
{
    public class ReferralServiceTestsFixture
    {
        public ReferralServiceTestsFixture()
        {

            ReferralRepositoryMock = new Mock<IReferralRepository>(MockBehavior.Strict);
            HashingManagerMock = new Mock<IHashingManager>(MockBehavior.Strict);

            Service = new ReferralService(
                ReferralRepositoryMock.Object,
                6,
                HashingManagerMock.Object,
                EmptyLogFactory.Instance);

            Referral = new Domain.Models.Referral()
            {
                CustomerId = CustomerId,
                Id = Guid.NewGuid().ToString("D"),
                ReferralCode = ReferralCode
            };

            SetupCalls();
        }

        public Mock<IReferralRepository> ReferralRepositoryMock;
        public Mock<IHashingManager> HashingManagerMock;
        public ReferralService Service;

        public Guid CustomerId = Guid.NewGuid();
        public string ReferralCode = "ref123";
        public bool Created = true;
        public int CurrentReferral = 0;
        public Domain.Models.Referral Referral;
        public List<Domain.Models.Referral> Referrals = new List<Domain.Models.Referral>();

        public void SetupCalls()
        {
            ReferralRepositoryMock.Setup(c => c.GetByCustomerIdAsync(CustomerId))
                .ReturnsAsync(() => Referral);

            ReferralRepositoryMock.Setup(c => c.GetByReferralCodeAsync(ReferralCode))
                .ReturnsAsync(() =>
                {
                    if (Referrals.Count == 0 || CurrentReferral + 1 > Referrals.Count)
                    {
                        return null;
                    }
                    var index = CurrentReferral;
                    CurrentReferral += 1;
                    return Referrals[index];
                });

            ReferralRepositoryMock.Setup(c => c.CreateIfNotExistAsync(It.IsAny<Domain.Models.Referral>()))
                .ReturnsAsync(() => Created);

            HashingManagerMock.Setup(c => c.GenerateBase(It.IsAny<string>()))
                .Returns(() => ReferralCode);
        }
    }
}
