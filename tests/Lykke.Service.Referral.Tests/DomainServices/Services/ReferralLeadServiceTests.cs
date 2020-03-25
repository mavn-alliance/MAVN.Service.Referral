using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Falcon.Common;
using Lykke.Service.AgentManagement.Client.Models.Agents;
using Lykke.Service.MAVNPropertyIntegration.Client.Models;
using Lykke.Service.Referral.Contract.Events;
using Lykke.Service.Referral.Domain.Entities;
using Lykke.Service.Referral.Domain.Exceptions;
using Lykke.Service.Referral.Domain.Models;
using Moq;
using Xunit;

namespace Lykke.Service.Referral.Tests.DomainServices.Services
{
    public class ReferralLeadServiceTests
    {
        [Fact]
        public async Task ShouldCreateReferral_WhenValidDataIsPassed()
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture();

            // Act
            await fixture.Service.CreateReferralLeadAsync(fixture.ReferralLead);

            // Assert
            
            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.CreateAsync(
                    It.Is<ReferralLeadEncrypted>(c =>
                        c.AgentId == fixture.AgentId &&
                        c.PhoneNumberHash != fixture.PhoneNumber)),
                Times.Once);
            
            fixture.AgentManagementServiceMock.Verify(
                x => x.Agents.GetByCustomerIdAsync(
                    It.Is<Guid>(c => c == fixture.ReferralLead.AgentId)),
                Times.Once);
            var phoneNumberE164 = PhoneUtils.GetE164FormattedNumber(fixture.ReferralLead.PhoneNumber, fixture.CountryCode.ToString());
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadConfirmRequestAsync(
                    It.Is<string>(c => c == fixture.ReferralLead.AgentId.ToString()),
                    It.Is<string>(c => c == phoneNumberE164),
                    It.Is<string>(c => c == fixture.ReferralLead.ConfirmationToken)),
                Times.Once);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadAlreadyConfirmedAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);

            fixture.LeadStateChangePublisherMock.Verify(
                x =>
                    x.PublishAsync(
                        It.Is<LeadStateChangedEvent>(
                            c =>
                                c.State == Contract.Enums.ReferralLeadState.Pending)),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowReferralLeadAlreadyExistException_IfConfirmedEmailLeadExists()
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture();
            fixture.ReferralLead.Email = "email@mail.com";

            fixture.ReferralLeadRepositoryMock.Setup(c => c.GetByEmailHashAsync(It.IsAny<string>()))
                .ReturnsAsync((string s) => new List<ReferralLeadEncrypted>
                {
                    new ReferralLeadEncrypted {State = ReferralLeadState.Confirmed}
                });
            
            // Act
            await Assert.ThrowsAsync<ReferralLeadAlreadyConfirmedException>(async () =>
            {
                await fixture.Service.CreateReferralLeadAsync(fixture.ReferralLead);
            });

            // Assert
            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
            
            fixture.AgentManagementServiceMock.Verify(
                x => x.Agents.GetByCustomerIdAsync(
                    It.IsAny<Guid>()),
                Times.Once);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadConfirmRequestAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadAlreadyConfirmedAsync(
                    It.Is<string>(c => fixture.ReferralLead.AgentId.ToString() == c),
                    It.Is<string>(c => fixture.ReferralLead.FirstName == c),
                    It.Is<string>(c => fixture.ReferralLead.LastName == c),
                    It.Is<string>(c => fixture.ReferralLead.PhoneNumber != c)),
                Times.Once);
            
            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.CreateAsync(It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
        }

        [Fact]
        public async Task ShouldThrowReferralLeadAlreadyExistException_IfConfirmedPhoneLeadExists()
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture();
            fixture.ReferralLead.PhoneNumber = "0881212838";
            
            fixture.ReferralLeadRepositoryMock.Setup(c => c.GetByEmailHashAsync(It.IsAny<string>()))
                .ReturnsAsync((string s) => new List<ReferralLeadEncrypted>
                {
                    new ReferralLeadEncrypted {State = ReferralLeadState.Confirmed}
                });
            
            // Act
            await Assert.ThrowsAsync<ReferralLeadAlreadyConfirmedException>(async () =>
            {
                await fixture.Service.CreateReferralLeadAsync(fixture.ReferralLead);
            });

            // Assert
            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
            
            fixture.AgentManagementServiceMock.Verify(
                x => x.Agents.GetByCustomerIdAsync(
                    It.IsAny<Guid>()),
                Times.Once);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadConfirmRequestAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadAlreadyConfirmedAsync(
                    It.Is<string>(c => fixture.ReferralLead.AgentId.ToString() == c),
                    It.Is<string>(c => fixture.ReferralLead.FirstName == c),
                    It.Is<string>(c => fixture.ReferralLead.LastName == c),
                    It.Is<string>(c => fixture.ReferralLead.PhoneNumber != c)),
                Times.Once);
            
            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.CreateAsync(It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
        }
        
        [Fact]
        public async Task ShouldThrowCustomerDoesNotExistException_IfInvalidAgentIdIsPassed()
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture();
            fixture.ShouldFindAgentsCustomerProfile = false;
            
            // Act
            await Assert.ThrowsAsync<CustomerDoesNotExistException>(async () =>
            {
                await fixture.Service.CreateReferralLeadAsync(fixture.ReferralLead);
            });

            // Assert
            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
            
            fixture.AgentManagementServiceMock.Verify(
                x => x.Agents.GetByCustomerIdAsync(
                    It.IsAny<Guid>()),
                Times.Never);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadConfirmRequestAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadAlreadyConfirmedAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
            
            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.CreateAsync(It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
        }
        
        [Theory]
        [InlineData(AgentStatus.None)]
        [InlineData(AgentStatus.NotAgent)]
        public async Task ShouldThrowCustomerNotApprovedAgentException_IfAgentNotApproved(AgentStatus status)
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture {AgentStatus = status};

            // Act
            await Assert.ThrowsAsync<CustomerNotApprovedAgentException>(async () =>
            {
                await fixture.Service.CreateReferralLeadAsync(fixture.ReferralLead);
            });

            // Assert
            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
            
            fixture.AgentManagementServiceMock.Verify(
                x => x.Agents.GetByCustomerIdAsync(
                    It.Is<Guid>(c => c == fixture.ReferralLead.AgentId)),
                Times.Once);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadConfirmRequestAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadAlreadyConfirmedAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
            
            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.CreateAsync(It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
        }

        [Fact]
        public async Task ShouldConfirmReferral_WhenValidDataIsPassed()
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture();
            
            var confirmToken = "3l2k3h4lk";

            var referralLead = fixture.ReferralLeads.Single(o => o.ConfirmationToken == confirmToken);

            // Act
            var result = await fixture.Service.ConfirmReferralLeadAsync(confirmToken);
            
            // Assert
            Assert.Equal(ReferralLeadState.Confirmed, result.State);

            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.UpdateAsync(
                    It.Is<ReferralLeadEncrypted>(
                        c =>
                            c.ConfirmationToken == confirmToken &&
                            c.State == referralLead.State &&
                            c.SalesforceId == referralLead.SalesforceId &&
                            c.ResponseStatus == referralLead.ResponseStatus)),
                Times.Once);
            
            fixture.PropertyIntegrationClientMock.Verify(
                x => x.Api.RegisterLeadAsync(
                    It.Is<LeadRegisterRequestModel>(
                        c =>
                            c.FirstName == fixture.FirstName &&
                            c.LastName == fixture.LastName &&
                            c.AgentSalesforceId == referralLead.AgentSalesforceId &&
                            c.LeadNote == fixture.Note)),
                Times.Once);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadSuccessfullyConfirmedAsync(
                    It.Is<string>(c => referralLead.AgentId.ToString() == c),
                    It.Is<string>(c => fixture.FirstName == c),
                    It.Is<string>(c => fixture.LastName == c),
                    It.Is<string>(c => fixture.PhoneNumber == c)),
                Times.Once);

            fixture.LeadStateChangePublisherMock.Verify(
                x =>
                    x.PublishAsync(
                        It.Is<LeadStateChangedEvent>(
                            c =>
                                c.State == Contract.Enums.ReferralLeadState.Confirmed)),
                Times.Once);
        }
        
        [Theory]
        [InlineData(ReferralLeadState.Confirmed)]
        [InlineData(ReferralLeadState.Approved)]
        public async Task ShouldNotDoAnything_WhenGivenReferralAlreadyConfirmed(ReferralLeadState state)
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture();
            var confirmToken = "3l2k3h4lk";

            var referral = fixture.ReferralLeads.Single(x => x.ConfirmationToken == confirmToken);

            referral.State = state;

            // Act
            var result = await fixture.Service.ConfirmReferralLeadAsync(confirmToken);
            
            // Assert
            Assert.Equal(state, result.State);

            // Assert
            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
            
            fixture.PropertyIntegrationClientMock.Verify(
                x => x.Api.RegisterLeadAsync(
                    It.IsAny<LeadRegisterRequestModel>()),
                Times.Never);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadSuccessfullyConfirmedAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ShouldThrowReferralLeadDoesNotExistException_IfWrongConfirmCode()
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture();
            var confirmToken = Guid.NewGuid().ToString();
            
            // Act
            await Assert.ThrowsAsync<ReferralDoesNotExistException>(async () =>
            {
                await fixture.Service.ConfirmReferralLeadAsync(confirmToken);
            });
            
            // Assert
            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
            
            fixture.PropertyIntegrationClientMock.Verify(
                x => x.Api.RegisterLeadAsync(
                    It.IsAny<LeadRegisterRequestModel>()),
                Times.Never);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadSuccessfullyConfirmedAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ShouldThrowReferralLeadAlreadyConfirmedException_IfLeadWithSamePhoneAlreadyConfirmed()
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture();
            var confirmToken = "3l2k3h4lk";

            var referral = fixture.ReferralLeads.Single(x => x.ConfirmationToken == confirmToken);

            var approvedReferral = fixture.ReferralLeads.First(x => x.State == ReferralLeadState.Confirmed);

            referral.PhoneNumberHash = approvedReferral.PhoneNumberHash;
            
            // Act
            await Assert.ThrowsAsync<ReferralAlreadyConfirmedException>(async () =>
            {
                await fixture.Service.ConfirmReferralLeadAsync(confirmToken);
            });
            
            // Assert
            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
            
            fixture.PropertyIntegrationClientMock.Verify(
                x => x.Api.RegisterLeadAsync(
                    It.IsAny<LeadRegisterRequestModel>()),
                Times.Never);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadSuccessfullyConfirmedAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ShouldThrowReferralLeadAlreadyConfirmedException_IfLeadWithSameEmailAlreadyConfirmed()
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture();
            var confirmToken = "3l2k3h4lk";

            var referral = fixture.ReferralLeads.Single(x => x.ConfirmationToken == confirmToken);

            var approvedReferral = fixture.ReferralLeads.First(x => x.State == ReferralLeadState.Approved);

            referral.EmailHash = approvedReferral.EmailHash;
            
            // Act
            await Assert.ThrowsAsync<ReferralAlreadyConfirmedException>(async () =>
            {
                await fixture.Service.ConfirmReferralLeadAsync(confirmToken);
            });
            
            // Assert
            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
            
            fixture.PropertyIntegrationClientMock.Verify(
                x => x.Api.RegisterLeadAsync(
                    It.IsAny<LeadRegisterRequestModel>()),
                Times.Never);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadSuccessfullyConfirmedAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ShouldThrowPropertyReferralFailed_WhenIntegrationClientThrowsAnException()
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture();
            var confirmToken = "3l2k3h4lk";
            var refferalLead = fixture.ReferralLeads.Single(o => o.ConfirmationToken == confirmToken);

            fixture.PropertyIntegrationClientMock
                .Setup(c => c.Api.RegisterLeadAsync(It.IsAny<LeadRegisterRequestModel>()))
                .ThrowsAsync(new ArgumentException());

            // Act
            await Assert.ThrowsAsync<ReferralLeadConfirmationFailedException>(async () =>
            {
                await fixture.Service.ConfirmReferralLeadAsync(confirmToken);
            });
            
            // Assert
            fixture.PropertyIntegrationClientMock.Verify(
                x => x.Api.RegisterLeadAsync(
                    It.Is<LeadRegisterRequestModel>(
                        c =>
                            c.FirstName == fixture.FirstName &&
                            c.LastName == fixture.LastName &&
                            c.AgentSalesforceId == refferalLead.AgentSalesforceId &&
                            c.LeadNote == fixture.Note)),
                Times.Once);
            
            fixture.ReferralLeadRepositoryMock.Verify(
                x =>
                    x.UpdateAsync(It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
            
            fixture.NotificationPublisherServiceMock.Verify(
                x => x.LeadSuccessfullyConfirmedAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }
        
        [Fact]
        public async Task ShouldApproveReferral_WhenValidDataIsPassed()
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture();
            var id = Guid.Parse("57e80137-984c-44f0-ad6f-b555d46cd934");
            var approvalDate = DateTime.UtcNow;

            var referral = fixture.ReferralLeads.Single(x => x.Id == id);

            referral.State = ReferralLeadState.Confirmed;
            
            // Act
            var result = await fixture.Service.ApproveReferralLeadAsync(id, approvalDate);
            
            // Assert
            Assert.Equal(ReferralLeadState.Approved, result.State);
            
            fixture.EventPublisher.Verify(
                x =>
                    x.PublishAsync(
                        It.Is<PropertyLeadApprovedReferralEvent>(
                            c =>
                                c.ReferrerId == "78ceb436-29a9-499c-92fc-ec77152e32d8" &&
                                c.TimeStamp == approvalDate)),
                Times.Once);

            fixture.LeadStateChangePublisherMock.Verify(
                x =>
                    x.PublishAsync(
                        It.Is<LeadStateChangedEvent>(
                            c =>
                                c.LeadId == id.ToString() &&
                                c.State == Contract.Enums.ReferralLeadState.Approved )),
                Times.Once);

            fixture.LeadStateChangePublisherMock.Verify(
                x =>
                    x.PublishAsync(
                        It.Is<LeadStateChangedEvent>(
                            c =>
                                c.LeadId == id.ToString() &&
                                c.State == Contract.Enums.ReferralLeadState.Approved)),
                Times.Once);

            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.UpdateAsync(
                    It.Is<ReferralLeadEncrypted>(
                        c =>
                            c.State == ReferralLeadState.Approved)),
                Times.Once);
        }
        
        [Fact]
        public async Task ShouldThrowReferralLeadDoesNotExistException_IfWrongReferId()
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture();
            var id = Guid.NewGuid();
            
            // Act
            await Assert.ThrowsAsync<ReferralDoesNotExistException>(async () =>
            {
                await fixture.Service.ApproveReferralLeadAsync(id, DateTime.UtcNow);
            });
            
            // Assert
            fixture.EventPublisher.Verify(
                x =>
                    x.PublishAsync(It.IsAny<PropertyLeadApprovedReferralEvent>()),
                Times.Never);

            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
        }
        
        [Theory]
        [InlineData(ReferralLeadState.Approved)]
        [InlineData(ReferralLeadState.Pending)]
        public async Task ShouldInvalidOperationException_IfReferStatusNotConfirmed(ReferralLeadState state)
        {
            // Arrange
            var fixture = new ReferralLeadServiceTestsFixture();
            var id = Guid.Parse("57e80137-984c-44f0-ad6f-b555d46cd934");
            var approvalDate = DateTime.UtcNow;

            var referral = fixture.ReferralLeads.Single(x => x.Id == id);

            referral.State = state;
            
            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await fixture.Service.ApproveReferralLeadAsync(id, approvalDate);
            });
            
            // Assert
            fixture.EventPublisher.Verify(
                x =>
                    x.PublishAsync(It.IsAny<PropertyLeadApprovedReferralEvent>()),
                Times.Never);

            fixture.ReferralLeadRepositoryMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<ReferralLeadEncrypted>()),
                Times.Never);
        }
    }
}
