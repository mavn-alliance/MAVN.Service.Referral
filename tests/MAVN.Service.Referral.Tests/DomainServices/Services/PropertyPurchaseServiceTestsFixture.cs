using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.Referral.Contract.Events;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.Domain.Services;
using MAVN.Service.Referral.DomainServices;
using MAVN.Service.Referral.DomainServices.Managers;
using MAVN.Service.Referral.DomainServices.Services;
using Moq;

namespace MAVN.Service.Referral.Tests.DomainServices.Services
{
    public class PropertyPurchaseServiceTestsFixture
    {
        public PropertyPurchaseServiceTestsFixture()
        {
            ReferralLeadRepositoryMock = new Mock<IReferralLeadRepository>(MockBehavior.Strict);
            PropertyPurchaseRepositorMock = new Mock<IPropertyPurchaseRepository>(MockBehavior.Strict);
            StakeServiceMock = new Mock<IStakeService>(MockBehavior.Strict);

            Service = new PropertyPurchaseService(
                ReferralLeadRepositoryMock.Object,
                PropertyPurchaseRepositorMock.Object,
                StakeServiceMock.Object,
                new CommissionManager(MapperHelper.CreateAutoMapper()));

            ReferralLead = new ReferralLeadEncrypted
            {
                Id = Guid.NewGuid(),
                AgentId = AgentId,
                PhoneNumberHash = PhoneNumber,
                EmailHash = Email,
                ConfirmationToken = ConfirmationToken,
                CampaignId = Guid.NewGuid()
            };

            ReferralLeadWithDetails = new ReferralLeadEncryptedWithDetails
            {
                Id = Guid.NewGuid(),
                AgentId = AgentId,
                PhoneNumberHash = PhoneNumber,
                EmailHash = Email,
                ConfirmationToken = ConfirmationToken
            };
            
            PropertyPurchase = new PropertyPurchase
            {
                Id = Guid.NewGuid(),
                ReferralLeadId = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            ReferralLeads = new List<ReferralLeadEncrypted> { ReferralLead };
            
            ReferralLeadsWithDetails = new List<ReferralLeadEncryptedWithDetails> { ReferralLeadWithDetails };
            
            PropertyPurchases = new List<PropertyPurchase>
            {
                PropertyPurchase,
                PropertyPurchase,
                PropertyPurchase
            };

            SetupCalls();
        }

        public Mock<IStakeService> StakeServiceMock { get; set; }

        public Mock<IReferralLeadRepository> ReferralLeadRepositoryMock;
        public Mock<IPropertyPurchaseRepository> PropertyPurchaseRepositorMock;
        public PropertyPurchaseService Service;

        public Guid AgentId = Guid.NewGuid();
        public string FirstName = "fname";
        public string LastName = "lname";
        public string Email = "email@mail.com";
        public string PhoneNumber = "number";
        public string Note = "note";
        public string SalesforceId = "salesforceId";
        public bool PropertyPurchaseExists = false;
        public string ConfirmationToken = Guid.NewGuid().ToString();
        public ReferralLeadEncrypted ReferralLead;
        public ReferralLeadEncryptedWithDetails ReferralLeadWithDetails;
        public PropertyPurchase PropertyPurchase;
        public IReadOnlyList<ReferralLeadEncrypted> ReferralLeads;
        public IReadOnlyList<ReferralLeadEncryptedWithDetails> ReferralLeadsWithDetails;
        public IReadOnlyList<PropertyPurchase> PropertyPurchases;
        public List<Domain.Models.Referral> Referrals = new List<Domain.Models.Referral>();

        public void SetupCalls()
        {
            var emptyList = new List<ReferralLeadEncrypted>();
            
            var fullList = new List<ReferralLeadEncrypted>();
            fullList.Add(ReferralLead);
            
            ReferralLeadRepositoryMock.Setup(c => c.CreateAsync(ReferralLead))
                .ReturnsAsync(() => ReferralLead);

            ReferralLeadRepositoryMock.Setup(c => c.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => ReferralLead);

            ReferralLeadRepositoryMock.Setup(c => c.GetByEmailHashAsync(It.IsAny<string>()))
                .ReturnsAsync((string s) => ReferralLead.EmailHash == s ? fullList : emptyList);

            ReferralLeadRepositoryMock.Setup(c => c.GetByPhoneNumberHashAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((int cc, string s) => ReferralLead.PhoneNumberHash == s ? fullList : emptyList);

            ReferralLeadRepositoryMock.Setup(c => c.GetByConfirmationTokenAsync(It.IsAny<string>()))
                .ReturnsAsync((string s) => ReferralLead.ConfirmationToken == s ? ReferralLead : null);

            ReferralLeadRepositoryMock.Setup(c => c.DoesExistAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => ReferralLead != null);

            ReferralLeadRepositoryMock.Setup(c => c.UpdateAsync(ReferralLead))
                .ReturnsAsync(() => ReferralLead);

            ReferralLeadRepositoryMock.Setup(c => c.GetForReferrerAsync(It.IsAny<Guid>(), null, null))
                .ReturnsAsync(() => ReferralLeadsWithDetails);
            
            PropertyPurchaseRepositorMock.Setup(c => c.InsertAsync(It.IsAny<PropertyPurchase>()))
                .Returns(Task.CompletedTask);

            PropertyPurchaseRepositorMock.Setup(c => c.GetAsync())
                .ReturnsAsync(() => PropertyPurchases);

            PropertyPurchaseRepositorMock.Setup(c => c.PropertyPurchaseExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => PropertyPurchaseExists);
        }
    }
}
