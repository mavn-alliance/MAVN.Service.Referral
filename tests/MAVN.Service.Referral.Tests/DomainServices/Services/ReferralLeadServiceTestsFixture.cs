using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.AgentManagement.Client;
using Lykke.Service.AgentManagement.Client.Models.Agents;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.CustomerProfile.Client.Models.Enums;
using Lykke.Service.CustomerProfile.Client.Models.Requests;
using Lykke.Service.CustomerProfile.Client.Models.Responses;
using Lykke.Service.Dictionaries.Client;
using Lykke.Service.Dictionaries.Client.Models.Salesforce;
using Lykke.Service.MAVNPropertyIntegration.Client;
using Lykke.Service.MAVNPropertyIntegration.Client.Models;
using MAVN.Service.Referral.Contract.Events;
using MAVN.Service.Referral.Domain.Entities;
using MAVN.Service.Referral.Domain.Managers;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.Domain.Services;
using MAVN.Service.Referral.DomainServices;
using MAVN.Service.Referral.DomainServices.Services;
using Moq;

namespace MAVN.Service.Referral.Tests.DomainServices.Services
{
    public class ReferralLeadServiceTestsFixture
    {
        public ReferralLeadServiceTestsFixture()
        {
            StakeServiceMock = new Mock<IStakeService>(MockBehavior.Strict);
            PropertyIntegrationClientMock = new Mock<IMAVNPropertyIntegrationClient>(MockBehavior.Strict);
            EventPublisher = new Mock<IRabbitPublisher<PropertyLeadApprovedReferralEvent>>(MockBehavior.Strict);
            ReferralLeadRepositoryMock = new Mock<IReferralLeadRepository>(MockBehavior.Strict);
            NotificationPublisherServiceMock = new Mock<INotificationPublisherService>(MockBehavior.Strict);
            SettingsServiceMock = new Mock<ISettingsService>(MockBehavior.Strict);
            HashingManagerMock = new Mock<IHashingManager>(MockBehavior.Strict);
            CustomerProfileMock = new Mock<ICustomerProfileClient>(MockBehavior.Strict);
            DictionariesClientMock = new Mock<IDictionariesClient>(MockBehavior.Strict);
            AgentManagementServiceMock = new Mock<IAgentManagementClient>(MockBehavior.Strict);
            PropertyPurchaseRepositoryMock = new Mock<IPropertyPurchaseRepository>(MockBehavior.Strict);
            LeadStateChangePublisherMock = new Mock<IRabbitPublisher<LeadStateChangedEvent>>(MockBehavior.Strict);

            Service = new ReferralLeadService(
                StakeServiceMock.Object,
                PropertyIntegrationClientMock.Object,
                EventPublisher.Object,
                ReferralLeadRepositoryMock.Object,
                NotificationPublisherServiceMock.Object,
                SettingsServiceMock.Object,
                HashingManagerMock.Object,
                AgentManagementServiceMock.Object,
                CustomerProfileMock.Object,
                DictionariesClientMock.Object,
                PropertyPurchaseRepositoryMock.Object,
                LeadStateChangePublisherMock.Object,
                MapperHelper.CreateAutoMapper(),
                EmptyLogFactory.Instance);

            ReferralLead = new ReferralLead
            {
                Id = Guid.NewGuid(),
                AgentId = AgentId,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                PhoneCountryCodeId = CountryCode,
                ConfirmationToken = ConfirmationToken,
                AgentSalesforceId = AgentSalesforceId,
                Note = Note
            };
            
            ReferralLeads = new List<ReferralLeadEncrypted>
            {
                new ReferralLeadEncrypted
                {
                    Id = Guid.NewGuid(),
                    AgentId = Guid.Parse("78ceb436-29a9-499c-92fc-ec77152e32d8"),
                    State = ReferralLeadState.Pending
                },
                new ReferralLeadEncrypted
                {
                    Id = Guid.NewGuid(),
                    State = ReferralLeadState.Pending
                },
                new ReferralLeadEncrypted
                {
                    Id = Guid.Parse("57e80137-984c-44f0-ad6f-b555d46cd934"),
                    AgentId = Guid.Parse("78ceb436-29a9-499c-92fc-ec77152e32d8"),
                    EmailHash = "email@gmail.com",
                    ConfirmationToken = "3l2k3h4lk",
                    State = ReferralLeadState.Pending,
                    PhoneNumberHash = "2384324092345",
                    PhoneCountryCodeId = 359,
                    AgentSalesforceId = "agentsfid"
                },
                new ReferralLeadEncrypted
                {
                    Id = Guid.NewGuid(),
                    AgentId = Guid.NewGuid(),
                    State = ReferralLeadState.Confirmed,
                    EmailHash = "email@mail.com",
                    PhoneNumberHash = "0881212838",
                    PhoneCountryCodeId = 359,
                    ConfirmationToken = "9l2l7a4lk"
                },
                new ReferralLeadEncrypted
                {
                    Id = Guid.NewGuid(),
                    AgentId = Guid.NewGuid(),
                    State = ReferralLeadState.Approved,
                    EmailHash = "another@yahoo.com",
                    PhoneNumberHash = "0884543421",
                    PhoneCountryCodeId = 359,
                    ConfirmationToken = "7a00a3a8p"
                },
                new ReferralLeadEncrypted
                {
                    Id = Guid.NewGuid(),
                    AgentId = Guid.NewGuid(),
                    State = ReferralLeadState.Approved,
                    EmailHash = "another-one@yahoo.com",
                    PhoneNumberHash = "0884543421",
                    PhoneCountryCodeId = 359,
                    ConfirmationToken = "1b9yhklj3"
                }
            };

            ReferralLeadsWithDetails = new List<ReferralLeadEncryptedWithDetails>
            {
                new ReferralLeadEncryptedWithDetails
                {
                    Id = Guid.NewGuid(),
                    AgentId = Guid.Parse("78ceb436-29a9-499c-92fc-ec77152e32d8"),
                    State = ReferralLeadState.Pending
                },
                new ReferralLeadEncryptedWithDetails
                {
                    Id = Guid.NewGuid(),
                    State = ReferralLeadState.Pending
                },
                new ReferralLeadEncryptedWithDetails
                {
                    Id = Guid.Parse("57e80137-984c-44f0-ad6f-b555d46cd934"),
                    AgentId = Guid.Parse("78ceb436-29a9-499c-92fc-ec77152e32d8"),
                    EmailHash = "email@gmail.com",
                    ConfirmationToken = "3l2k3h4lk",
                    State = ReferralLeadState.Pending,
                    PhoneNumberHash = "2384324092345",
                    PhoneCountryCodeId = 359,
                    AgentSalesforceId = "agentsfid"
                },
                new ReferralLeadEncryptedWithDetails
                {
                    Id = Guid.NewGuid(),
                    AgentId = Guid.NewGuid(),
                    State = ReferralLeadState.Confirmed,
                    EmailHash = "email@mail.com",
                    PhoneNumberHash = "0881212838",
                    PhoneCountryCodeId = 359,
                    ConfirmationToken = "9l2l7a4lk"
                },
                new ReferralLeadEncryptedWithDetails
                {
                    Id = Guid.NewGuid(),
                    AgentId = Guid.NewGuid(),
                    State = ReferralLeadState.Approved,
                    EmailHash = "another@yahoo.com",
                    PhoneNumberHash = "0884543421",
                    PhoneCountryCodeId = 359,
                    ConfirmationToken = "7a00a3a8p"
                },
                new ReferralLeadEncryptedWithDetails
                {
                    Id = Guid.NewGuid(),
                    AgentId = Guid.NewGuid(),
                    State = ReferralLeadState.Approved,
                    EmailHash = "another-one@yahoo.com",
                    PhoneNumberHash = "0884543421",
                    PhoneCountryCodeId = 359,
                    ConfirmationToken = "1b9yhklj3"
                }
            };

            ReferralLeadProfileResponse = new ReferralLeadProfileResponse
            {
                ErrorCode = ReferralLeadProfileErrorCodes.None,
                Data = new ReferralLeadProfile
                {
                    ReferralLeadId = Guid.NewGuid(),
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    PhoneNumber = PhoneNumber,
                    Note = Note
                }
            };
            
            SetupCalls();
        }

        public Mock<IStakeService> StakeServiceMock { get; set; }

        public Mock<IDictionariesClient> DictionariesClientMock { get; set; }

        public Mock<IMAVNPropertyIntegrationClient> PropertyIntegrationClientMock { get; set; }

        public Mock<IReferralLeadRepository> ReferralLeadRepositoryMock;

        public Mock<IPropertyPurchaseRepository> PropertyPurchaseRepositoryMock;

        public Mock<INotificationPublisherService> NotificationPublisherServiceMock { get; set; }
        public Mock<ISettingsService> SettingsServiceMock { get; set; }
        public Mock<IHashingManager> HashingManagerMock { get; set; }
        
        public Mock<IAgentManagementClient> AgentManagementServiceMock { get; set; }
        
        public Mock<ICustomerProfileClient> CustomerProfileMock { get; set; }
        
        public Mock<IRabbitPublisher<PropertyLeadApprovedReferralEvent>> EventPublisher { get; set; }

        public Mock<IRabbitPublisher<LeadStateChangedEvent>> LeadStateChangePublisherMock { get; set; }

        public bool IsStakeEnabled { get; set; } = false;

        public ReferralLeadService Service;

        public Guid AgentId = Guid.NewGuid();
        public string FirstName = "fname";
        public string LastName = "lname";
        public string Email = "email@email.com";
        public string PhoneNumber = "00 00 000";
        public string Note = "note";
        public string SalesforceId = "salesforceId";
        public string AgentSalesforceId = "agentsalesforceId";
        public string ResponseStatus = "success";
        public string ConfirmationToken = Guid.NewGuid().ToString().Substring(0, 6);
        public ReferralLead ReferralLead;
        public IReadOnlyList<ReferralLeadEncrypted> ReferralLeads;
        public IReadOnlyList<ReferralLeadEncryptedWithDetails> ReferralLeadsWithDetails;
        public AgentStatus AgentStatus = AgentStatus.ApprovedAgent;
        public bool ShouldFindAgentsCustomerProfile = true;
        public int CountryCode = 359;
        
        public ReferralLeadProfileResponse ReferralLeadProfileResponse;

        public void SetupCalls()
        {
            ReferralLeadRepositoryMock.Setup(c => c.CreateAsync(It.IsAny<ReferralLeadEncrypted>()))
                .ReturnsAsync((ReferralLeadEncrypted c) => c);

            ReferralLeadRepositoryMock.Setup(c => c.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid c) => ReferralLeads.SingleOrDefault(x => c == x.Id));

            ReferralLeadRepositoryMock.Setup(c => c.DoesExistAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => ReferralLead != null);

            ReferralLeadRepositoryMock.Setup(c => c.UpdateAsync(It.IsAny<ReferralLeadEncrypted>()))
                .ReturnsAsync((ReferralLeadEncrypted c) => c);

            ReferralLeadRepositoryMock.Setup(c => c.GetForReferrerAsync(It.IsAny<Guid>(), null, null))
                .ReturnsAsync(() => ReferralLeadsWithDetails);

            ReferralLeadRepositoryMock.Setup(c => c.GetByEmailHashAsync(It.IsAny<string>()))
                .ReturnsAsync((string s) => ReferralLeads.Where(x => x.EmailHash == s).ToList());

            ReferralLeadRepositoryMock.Setup(c => c.GetByPhoneNumberHashAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((int cc, string s) => ReferralLeads.Where(x => x.PhoneNumberHash == s).ToList());

            ReferralLeadRepositoryMock.Setup(c => c.GetByConfirmationTokenAsync(It.IsAny<string>()))
                .ReturnsAsync((string s) => ReferralLeads.SingleOrDefault(x => x.ConfirmationToken == s));

            HashingManagerMock.Setup(c => c.GenerateBase(It.IsAny<string>()))
                .Returns(() => ConfirmationToken);

            SettingsServiceMock.Setup(c => c.GetLeadConfirmationTokenLength())
                .Returns(() => 6);

            NotificationPublisherServiceMock.Setup(c =>
                    c.LeadConfirmRequestAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.CompletedTask);

            NotificationPublisherServiceMock.Setup(c =>
                    c.LeadAlreadyConfirmedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(() => Task.CompletedTask);

            NotificationPublisherServiceMock.Setup(c =>
                    c.LeadSuccessfullyConfirmedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .Returns(() => Task.CompletedTask);

            PropertyIntegrationClientMock.Setup(c => c.Api.RegisterLeadAsync(It.IsAny<LeadRegisterRequestModel>()))
                .ReturnsAsync(() => new LeadRegisterResponseModel
                {
                    ErrorCode = "",
                    SalesforceId = SalesforceId,
                    Status = ResponseStatus
                });

            AgentManagementServiceMock.Setup(c => c.Agents.GetByCustomerIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => new AgentModel
                {
                    Status = AgentStatus
                });

            EventPublisher.Setup(c => c.PublishAsync(It.IsAny<PropertyLeadApprovedReferralEvent>()))
                .Returns(() => Task.CompletedTask);

            LeadStateChangePublisherMock.Setup(c => c.PublishAsync(It.IsAny<LeadStateChangedEvent>()))
                .Returns(() => Task.CompletedTask);

            CustomerProfileMock.Setup(c => c.CustomerProfiles.GetByCustomerIdAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(() =>  new CustomerProfileResponse
                {
                    Profile = new CustomerProfile.Client.Models.Responses.CustomerProfile(),
                    ErrorCode = ShouldFindAgentsCustomerProfile ? CustomerProfileErrorCodes.None : CustomerProfileErrorCodes.CustomerProfileDoesNotExist
                });

            CustomerProfileMock.Setup(c => c.ReferralLeadProfiles.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid referralLeadId) => ReferralLeadProfileResponse);

            CustomerProfileMock.Setup(c => c.ReferralLeadProfiles.AddAsync(It.IsAny<ReferralLeadProfileRequest>()))
                .ReturnsAsync((ReferralLeadProfileRequest request) => new ReferralLeadProfileResponse
                {
                    ErrorCode = ReferralLeadProfileErrorCodes.None
                });

            DictionariesClientMock.Setup(c => c.Salesforce.GetCountryPhoneCodeByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(() =>
                    new CountryPhoneCodeModel {Id = 359, Code = "359", CountryName = "Bulgaria", IsoCode = "+359"}
                );

            StakeServiceMock.Setup(c => c.GetReferralStake(It.IsAny<Guid?>(), It.IsAny<string>()))
                .ReturnsAsync(() => new ReferralStake());

            StakeServiceMock.Setup(c => c.SetStake(It.IsAny<ReferralStake>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
        }
    }
}
