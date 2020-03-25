using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Falcon.Common;
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
using Lykke.Service.Referral.Contract.Events;
using Lykke.Service.Referral.Domain.Entities;
using Lykke.Service.Referral.Domain.Managers;
using Lykke.Service.Referral.Domain.Models;
using Lykke.Service.Referral.Domain.Repositories;
using Lykke.Service.Referral.Domain.Services;
using Lykke.Service.Referral.DomainServices;
using Lykke.Service.Referral.DomainServices.Extensions;
using Lykke.Service.Referral.DomainServices.Services;
using Moq;
using Xunit;

namespace Lykke.Service.Referral.Tests.DomainServices.Services
{
    public class ReferralLeadServiceNewTests
    {
        private readonly Mock<IStakeService> _stakeServiceMock =
            new Mock<IStakeService>();

        private readonly Mock<IMAVNPropertyIntegrationClient> _propertyIntegrationClientMock =
            new Mock<IMAVNPropertyIntegrationClient>();

        private readonly Mock<IRabbitPublisher<PropertyLeadApprovedReferralEvent>>
            _propertyLeadApprovedReferralPublisherMock =
                new Mock<IRabbitPublisher<PropertyLeadApprovedReferralEvent>>();

        private readonly Mock<IReferralLeadRepository> _referralLeadRepositoryMock =
            new Mock<IReferralLeadRepository>();

        private readonly Mock<INotificationPublisherService> _notificationPublisherServiceMock =
            new Mock<INotificationPublisherService>();

        private readonly Mock<ISettingsService> _settingsServiceMock =
            new Mock<ISettingsService>();

        private readonly Mock<IHashingManager> _hashingManagerMock =
            new Mock<IHashingManager>();

        private readonly Mock<IAgentManagementClient> _agentManagementClientMock =
            new Mock<IAgentManagementClient>();

        private readonly Mock<ICustomerProfileClient> _customerProfileClientMock =
            new Mock<ICustomerProfileClient>();

        private readonly Mock<IDictionariesClient> _dictionariesClientMock =
            new Mock<IDictionariesClient>();

        private readonly Mock<IPropertyPurchaseRepository> _propertyPurchaseRepositoryMock =
            new Mock<IPropertyPurchaseRepository>();

        private readonly Mock<IRabbitPublisher<LeadStateChangedEvent>> _leadStateChangedPublisherMock =
            new Mock<IRabbitPublisher<LeadStateChangedEvent>>();

        private readonly Guid _referralId = Guid.NewGuid();

        private readonly string _customerId = Guid.NewGuid().ToString();

        private readonly CountryPhoneCodeModel _countryPhoneCode = new CountryPhoneCodeModel {Id = 1, IsoCode = "+7"};

        private readonly ReferralLeadProfile _referralLeadProfile;

        private readonly AgentModel _agent;

        private readonly List<ReferralLeadEncrypted> _referralLeadsEncrypted = new List<ReferralLeadEncrypted>();

        private readonly ReferralLead _referralLeadCreateInfo;

        private readonly int _confirmationTokenLength = 3;

        private readonly IReferralLeadService _service;

        public ReferralLeadServiceNewTests()
        {
            _referralLeadProfile = new ReferralLeadProfile
            {
                ReferralLeadId = _referralId,
                FirstName = "first name",
                LastName = "last name",
                PhoneNumber = "+70000000000",
                Email = "a@a.com",
                Note = "note"
            };

            _referralLeadCreateInfo = new ReferralLead
            {
                FirstName = _referralLeadProfile.FirstName,
                LastName = _referralLeadProfile.LastName,
                PhoneCountryCodeId = _countryPhoneCode.Id,
                PhoneNumber = "000 000 00 00",
                Email = _referralLeadProfile.Email,
                Note = _referralLeadProfile.Note,
                AgentId = Guid.Parse(_customerId)
            };

            _agent = new AgentModel
            {
                CustomerId = Guid.Parse(_customerId),
                Status = AgentStatus.ApprovedAgent,
                SalesforceId = "SalesforceId"
            };

            _customerProfileClientMock.Setup(o => o.CustomerProfiles.GetByCustomerIdAsync(
                    It.Is<string>(customerId => customerId == _customerId), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync((string customerId, bool includeNonVerified, bool includeDeactivated) => new CustomerProfileResponse
                {
                    ErrorCode = CustomerProfileErrorCodes.None
                });

            _customerProfileClientMock
                .Setup(o => o.ReferralLeadProfiles.AddAsync(It.IsAny<ReferralLeadProfileRequest>()))
                .ReturnsAsync((ReferralLeadProfileRequest request) => new ReferralLeadProfileResponse());

            _customerProfileClientMock.Setup(o => o.ReferralLeadProfiles.GetByIdAsync(
                    It.Is<Guid>(referralId => referralId == _referralId)))
                .ReturnsAsync((Guid referralId) => new ReferralLeadProfileResponse
                {
                    ErrorCode = ReferralLeadProfileErrorCodes.None, Data = _referralLeadProfile
                });

            _dictionariesClientMock.Setup(o => o.Salesforce.GetCountryPhoneCodeByIdAsync(
                    It.Is<int>(id => id == _countryPhoneCode.Id)))
                .ReturnsAsync((int id) => _countryPhoneCode);

            _agentManagementClientMock.Setup(o => o.Agents.GetByCustomerIdAsync(
                    It.Is<Guid>(customerId => customerId == Guid.Parse(_customerId))))
                .ReturnsAsync((Guid customerId) => _agent);

            _referralLeadRepositoryMock.Setup(o => o.CreateAsync(It.IsAny<ReferralLeadEncrypted>()))
                .ReturnsAsync((ReferralLeadEncrypted referralLeadEncrypted) =>
                {
                    referralLeadEncrypted.Id = _referralId;
                    referralLeadEncrypted.CreationDateTime = DateTime.UtcNow;

                    return referralLeadEncrypted;
                });

            _referralLeadRepositoryMock.Setup(o => o.GetByEmailHashAsync(It.IsAny<string>()))
                .ReturnsAsync((string emailHash) =>
                    _referralLeadsEncrypted.Where(o => o.EmailHash == emailHash).ToList());

            _referralLeadRepositoryMock.Setup(o => o.GetByPhoneNumberHashAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((int countryPhoneCodeId, string phoneNumberHash) =>
                    _referralLeadsEncrypted.Where(o => o.PhoneNumberHash == phoneNumberHash &&
                                                       o.PhoneCountryCodeId == countryPhoneCodeId).ToList());

            _referralLeadRepositoryMock.Setup(o => o.GetApprovedAsync())
                .ReturnsAsync(() => _referralLeadsEncrypted.Where(o => o.State == ReferralLeadState.Approved).ToList());

            _hashingManagerMock.Setup(o => o.GenerateBase(It.IsAny<string>()))
                .Returns((string value) => value);

            _settingsServiceMock.Setup(o => o.GetLeadConfirmationTokenLength())
                .Returns(() => _confirmationTokenLength);

            _service = new ReferralLeadService(
                _stakeServiceMock.Object,
                _propertyIntegrationClientMock.Object,
                _propertyLeadApprovedReferralPublisherMock.Object,
                _referralLeadRepositoryMock.Object,
                _notificationPublisherServiceMock.Object,
                _settingsServiceMock.Object,
                _hashingManagerMock.Object,
                _agentManagementClientMock.Object,
                _customerProfileClientMock.Object,
                _dictionariesClientMock.Object,
                _propertyPurchaseRepositoryMock.Object,
                _leadStateChangedPublisherMock.Object,
                MapperHelper.CreateAutoMapper(),
                EmptyLogFactory.Instance);
        }

        [Fact]
        public async Task Save_Encrypted_Sensitive_Data_Of_Referral_Lead_While_Creating()
        {
            // arrange

            var profileFixture = new CustomerProfileResponse()
            {
                Profile = new CustomerProfile.Client.Models.Responses.CustomerProfile()
                {
                    Email = "test@mail.bg"
                },
                ErrorCode = CustomerProfileErrorCodes.None
            };

            _customerProfileClientMock.Setup(c => c.CustomerProfiles.GetByCustomerIdAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(profileFixture);

            var emailHash = _referralLeadCreateInfo.Email.ToSha256Hash();

            var phoneNumberE164 =
                PhoneUtils.GetE164FormattedNumber(_referralLeadCreateInfo.PhoneNumber, _countryPhoneCode.IsoCode);

            var phoneNumberHash = phoneNumberE164.ToSha256Hash();

            // act 

            await _service.CreateReferralLeadAsync(_referralLeadCreateInfo);

            // assert

            _referralLeadRepositoryMock.Verify(o => o.CreateAsync(
                    It.Is<ReferralLeadEncrypted>(referralLeadEncrypted =>
                        referralLeadEncrypted.EmailHash == emailHash &&
                        referralLeadEncrypted.PhoneNumberHash == phoneNumberHash &&
                        referralLeadEncrypted.PhoneCountryCodeId == _countryPhoneCode.Id &&
                        referralLeadEncrypted.AgentId == _referralLeadCreateInfo.AgentId &&
                        referralLeadEncrypted.AgentSalesforceId == _agent.SalesforceId &&
                        referralLeadEncrypted.State == ReferralLeadState.Pending)),
                Times.Once);
        }

        [Fact]
        public async Task Create_Referral_Lead_Profile_While_Creating()
        {
            // arrange

            var phoneNumberE164 =
                PhoneUtils.GetE164FormattedNumber(_referralLeadCreateInfo.PhoneNumber, _countryPhoneCode.IsoCode);

            var profileFixture = new CustomerProfileResponse
            {
                Profile = new CustomerProfile.Client.Models.Responses.CustomerProfile
                {
                    Email = "test@mail.bg"
                },
                ErrorCode = CustomerProfileErrorCodes.None
            };

            _customerProfileClientMock.Setup(c => c.CustomerProfiles.GetByCustomerIdAsync(It.IsAny<string>(), false, false))
                .ReturnsAsync(profileFixture);

           // act 

           await _service.CreateReferralLeadAsync(_referralLeadCreateInfo);

            // assert

            _customerProfileClientMock.Verify(o => o.ReferralLeadProfiles.AddAsync(
                    It.Is<ReferralLeadProfileRequest>(request =>
                        request.ReferralLeadId == _referralId &&
                        request.FirstName == _referralLeadCreateInfo.FirstName &&
                        request.LastName == _referralLeadCreateInfo.LastName &&
                        request.Email == _referralLeadCreateInfo.Email &&
                        request.PhoneNumber == phoneNumberE164 &&
                        request.Note == _referralLeadCreateInfo.Note
                    )),
                Times.Once);
        }

        [Fact]
        public async Task Get_Approved_Decrypted_Referral_Lead()
        {
            // arrange

            _referralLeadsEncrypted.Add(
                new ReferralLeadEncrypted {Id = _referralId, State = ReferralLeadState.Approved});

            // act

            var referralLeads = await _service.GetApprovedLeadsAsync();

            var referralLead = referralLeads.Single();

            // assert

            Assert.True(referralLead.FirstName == _referralLeadProfile.FirstName &&
                        referralLead.LastName == _referralLeadProfile.LastName &&
                        referralLead.Email == _referralLeadProfile.Email &&
                        referralLead.PhoneNumber == _referralLeadProfile.PhoneNumber &&
                        referralLead.Note == _referralLeadProfile.Note);
        }
    }
}
