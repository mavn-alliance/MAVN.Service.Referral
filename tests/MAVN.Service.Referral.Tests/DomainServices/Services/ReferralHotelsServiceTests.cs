using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.Campaign.Client;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CurrencyConvertor.Client.Models.Enums;
using Lykke.Service.CurrencyConvertor.Client.Models.Responses;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.CustomerProfile.Client.Models.Enums;
using Lykke.Service.CustomerProfile.Client.Models.Requests;
using Lykke.Service.CustomerProfile.Client.Models.Responses;
using Lykke.Service.PartnerManagement.Client;
using Lykke.Service.PartnerManagement.Client.Models.Partner;
using MAVN.Service.Referral.Contract.Events;
using MAVN.Service.Referral.Domain.Exceptions;
using MAVN.Service.Referral.Domain.Managers;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.Domain.Services;
using MAVN.Service.Referral.DomainServices;
using MAVN.Service.Referral.DomainServices.Extensions;
using MAVN.Service.Referral.DomainServices.Services;
using Moq;
using Xunit;

namespace MAVN.Service.Referral.Tests.DomainServices.Services
{
    public class ReferralHotelsServiceTests
    {
        private readonly Mock<IStakeService> _stakeServiceMock =
            new Mock<IStakeService>();

        private readonly Mock<ICustomerProfileClient> _customerProfileClientMock =
            new Mock<ICustomerProfileClient>();

        private readonly Mock<ICampaignClient> _campaignClientMock =
            new Mock<ICampaignClient>(MockBehavior.Strict);

        private readonly Mock<IPartnerManagementClient> _partnerManagementClientMock =
            new Mock<IPartnerManagementClient>(MockBehavior.Strict);

        private readonly Mock<ICurrencyConvertorClient> _currencyConverterClientMock =
            new Mock<ICurrencyConvertorClient>(MockBehavior.Strict);

        private readonly Mock<IRabbitPublisher<HotelReferralUsedEvent>> _rabbitPublisherMock =
            new Mock<IRabbitPublisher<HotelReferralUsedEvent>>();

        private readonly Mock<INotificationPublisherService> _notificationPublisherServiceMock =
            new Mock<INotificationPublisherService>();

        private readonly Mock<IReferralHotelsRepository> _referralHotelsRepositoryMock =
            new Mock<IReferralHotelsRepository>();

        private readonly Mock<ISettingsService> _settingsServiceMock =
            new Mock<ISettingsService>();

        private readonly Mock<IHashingManager> _hashingManagerMock =
            new Mock<IHashingManager>();

        private readonly TimeSpan _referralExpirationPeriod = TimeSpan.FromDays(1);
        private readonly TimeSpan _referralLimitPeriod = TimeSpan.FromHours(1);
        private readonly int _referralLimit = 3;
        private readonly int _confirmationTokenLength = 3;

        private readonly Guid _referralId = Guid.NewGuid();
        private readonly string _email = "a@a.com";
        private readonly string _customerEmail = "b@b.com";
        private readonly string _customerId = Guid.NewGuid().ToString();

        private readonly List<ReferralHotelEncrypted> _referralHotelsEncrypted =
            new List<ReferralHotelEncrypted>();

        private readonly IReferralHotelsService _service;

        public ReferralHotelsServiceTests()
        {
            _customerProfileClientMock.Setup(o => o.CustomerProfiles
                    .GetByCustomerIdAsync(It.Is<string>(customerId => customerId == _customerId), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync((string customerId, bool includeNonVerified, bool includeDeactivated) => new CustomerProfileResponse
                {
                    ErrorCode = CustomerProfileErrorCodes.None,
                    Profile = new CustomerProfile
                    {
                        Email = _customerEmail,
                        CustomerId = customerId
                    }
                });

            _customerProfileClientMock.Setup(o => o.CustomerProfiles
                    .GetByCustomerIdAsync(It.Is<string>(customerId => customerId != _customerId), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync((string customerId, bool includeNonVerified, bool includeDeactivated) => new CustomerProfileResponse
                {
                    ErrorCode = CustomerProfileErrorCodes.CustomerProfileDoesNotExist
                });

            _customerProfileClientMock
                .Setup(o => o.ReferralHotelProfiles.AddAsync(It.IsAny<ReferralHotelProfileRequest>()))
                .ReturnsAsync((ReferralHotelProfileRequest referralHotelProfileRequest) =>
                    new ReferralHotelProfileResponse { ErrorCode = ReferralHotelProfileErrorCodes.None });

            _customerProfileClientMock.Setup(o => o.ReferralHotelProfiles
                    .GetByIdAsync(It.Is<Guid>(referralId => referralId == _referralId)))
                .ReturnsAsync((Guid referralId) => new ReferralHotelProfileResponse
                {
                    ErrorCode = ReferralHotelProfileErrorCodes.None,
                    Data = new ReferralHotelProfile { ReferralHotelId = _referralId, Email = _email }
                });

            _customerProfileClientMock.Setup(o => o.ReferralHotelProfiles
                    .GetByIdAsync(It.Is<Guid>(referralId => referralId != _referralId)))
                .ReturnsAsync((Guid referralId) => new ReferralHotelProfileResponse
                {
                    ErrorCode = ReferralHotelProfileErrorCodes.ReferralHotelProfileDoesNotExist
                });

            _referralHotelsRepositoryMock.Setup(o => o.GetByReferrerIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string referralId) =>
                    _referralHotelsEncrypted.Where(o => o.ReferrerId == referralId).ToList());

            _referralHotelsRepositoryMock.Setup(o =>
                    o.GetByEmailHashAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string emailHash, string partnerId, string location) =>
                {
                    var expression = _referralHotelsEncrypted.Where(o => o.EmailHash == emailHash);

                    if (!string.IsNullOrEmpty(partnerId))
                        expression = expression.Where(o => o.PartnerId == partnerId);

                    if (!string.IsNullOrEmpty(location))
                        expression = expression.Where(o => o.Location == location);

                    return expression.ToList();
                });

            _referralHotelsRepositoryMock.Setup(o => o.CreateAsync(It.IsAny<ReferralHotelEncrypted>()))
                .ReturnsAsync((ReferralHotelEncrypted referralHotelEncrypted) =>
                {
                    referralHotelEncrypted.Id = _referralId.ToString();
                    referralHotelEncrypted.CreationDateTime = DateTime.UtcNow;

                    return referralHotelEncrypted;
                });

            _referralHotelsRepositoryMock.Setup(o => o.UpdateAsync(It.IsAny<ReferralHotelEncrypted>()))
                .ReturnsAsync((ReferralHotelEncrypted referralHotelEncrypted) => referralHotelEncrypted);

            _referralHotelsRepositoryMock.Setup(o => o.GetByConfirmationTokenAsync(It.IsAny<string>()))
                .ReturnsAsync((string confirmationToken) =>
                    _referralHotelsEncrypted.SingleOrDefault(o => o.ConfirmationToken == confirmationToken));

            _currencyConverterClientMock.Setup(o => o.Converter.ConvertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
                .ReturnsAsync(new ConverterResponse() { ErrorCode = ConverterErrorCode.None });

            _partnerManagementClientMock.Setup(o => o.Partners.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new PartnerDetailsModel());

            _hashingManagerMock.Setup(o => o.GenerateBase(It.IsAny<string>()))
                .Returns((string value) => value);

            _settingsServiceMock.Setup(o => o.GetLeadConfirmationTokenLength())
                .Returns(() => _confirmationTokenLength);

            _service = new ReferralHotelsService(
                _stakeServiceMock.Object,
                _customerProfileClientMock.Object,
                _currencyConverterClientMock.Object,
                _campaignClientMock.Object,
                _partnerManagementClientMock.Object,
                _rabbitPublisherMock.Object,
                _notificationPublisherServiceMock.Object,
                _referralHotelsRepositoryMock.Object,
                _settingsServiceMock.Object,
                _hashingManagerMock.Object,
                _referralExpirationPeriod,
                _referralLimitPeriod,
                _referralLimit,
                MapperHelper.CreateAutoMapper(),
                "AED",
                EmptyLogFactory.Instance);
        }

        [Fact]
        public async Task Save_Encrypted_Sensitive_Data_Of_Referral_Hotel_While_Creating()
        {
            // act 

            await _service.CreateAsync(_email, _customerId, null, 123, "", "");

            // assert

            _referralHotelsRepositoryMock.Verify(o => o.CreateAsync(
                    It.Is<ReferralHotelEncrypted>(referralHotelEncrypted =>
                        referralHotelEncrypted.EmailHash == _email.ToSha256Hash())),
                Times.Once);
        }

        [Fact]
        public async Task Create_Referral_Hotel_Profile_While_Creating_Referral_Hotel()
        {
            // act 

            await _service.CreateAsync(_email, _customerId, null, 123, "", "");

            // assert

            _customerProfileClientMock.Verify(o => o.ReferralHotelProfiles.AddAsync(
                    It.Is<ReferralHotelProfileRequest>(request =>
                        request.ReferralHotelId == _referralId &&
                        request.Email == _email)),
                Times.Once);
        }

        [Fact]
        public async Task Publish_Referral_Hotel_Request_While_Creating_Referral_Hotel()
        {
            // act 

            var referralHotel = await _service.CreateAsync(_email, _customerId, null, 123, "", "");

            // assert

            _notificationPublisherServiceMock.Verify(o => o.HotelReferralConfirmRequestAsync(
                    It.Is<string>(referralId => referralId == referralHotel.ReferrerId),
                    It.Is<string>(email => email == referralHotel.Email),
                    It.Is<TimeSpan>(referralExpirationPeriod => referralExpirationPeriod == _referralExpirationPeriod),
                    It.Is<string>(confirmationToken => confirmationToken == referralHotel.ConfirmationToken)),
                Times.Once);
        }

        [Fact]
        public async Task Do_Not_Create_Referral_Hotel_If_Customer_Profile_Does_Not_Exist()
        {
            // arrange

            var referrerId = Guid.NewGuid().ToString();

            // act 

            var task = _service.CreateAsync(_email, referrerId, null, 123, "", "");

            // assert

            await Assert.ThrowsAsync<CustomerDoesNotExistException>(async () => await task);
        }

        [Fact]
        public async Task Do_Not_Create_Referral_Hotel_If_Limit_Exceeded()
        {
            // arrange

            for (var i = 0; i < _referralLimit + 1; i++)
            {
                _referralHotelsEncrypted.Add(new ReferralHotelEncrypted
                {
                    ReferrerId = _customerId,
                    CreationDateTime = DateTime.UtcNow
                });
            }

            // act 

            var task = _service.CreateAsync(_email, _customerId, null, 123, "", "");

            // assert

            await Assert.ThrowsAsync<ReferralHotelLimitExceededException>(async () => await task);
        }

        [Fact]
        public async Task Do_Not_Create_Referral_Hotel_If_Confirmed_Already_Exists()
        {
            // arrange

            _referralHotelsEncrypted.Add(new ReferralHotelEncrypted
            {
                EmailHash = _email.ToSha256Hash(),
                State = ReferralHotelState.Confirmed,
                ExpirationDateTime = DateTime.UtcNow.AddHours(1)
            });

            // act 

            var task = _service.CreateAsync(_email, _customerId, null, 123, "", "");

            // assert

            await Assert.ThrowsAsync<ReferralAlreadyConfirmedException>(async () => await task);
        }

        [Fact]
        public async Task Return_Confirmed_Referral_Hotel_If_Already_Exists()
        {
            // arrange

            var confirmationToken = "token";

            _referralHotelsEncrypted.Add(new ReferralHotelEncrypted
            {
                Id = _referralId.ToString(),
                ConfirmationToken = confirmationToken,
                State = ReferralHotelState.Confirmed
            });

            // act 

            await _service.ConfirmAsync(confirmationToken);

            // assert

            _referralHotelsRepositoryMock.Verify(o => o.UpdateAsync(It.IsAny<ReferralHotelEncrypted>()), Times.Never);
        }

        [Fact]
        public async Task Set_Referral_Hotel_Status_To_Confirmed_While_Confirming()
        {
            // arrange

            var confirmationToken = "token";

            _referralHotelsEncrypted.Add(new ReferralHotelEncrypted
            {
                Id = _referralId.ToString(),
                EmailHash = "hash",
                ConfirmationToken = confirmationToken,
                State = ReferralHotelState.Pending,
                ExpirationDateTime = DateTime.UtcNow.AddHours(1)
            });

            // act 

            await _service.ConfirmAsync(confirmationToken);

            // assert

            _referralHotelsRepositoryMock.Verify(o => o.UpdateAsync(
                    It.Is<ReferralHotelEncrypted>(referralHotelEncrypted =>
                        referralHotelEncrypted.State == ReferralHotelState.Confirmed)),
                Times.Once);
        }

        [Fact]
        public async Task Set_Referral_Hotel_Status_To_Expired_While_Confirming()
        {
            // arrange

            var confirmationToken = "token";

            _referralHotelsEncrypted.Add(new ReferralHotelEncrypted
            {
                Id = _referralId.ToString(),
                EmailHash = "hash",
                ConfirmationToken = confirmationToken,
                State = ReferralHotelState.Pending,
                ExpirationDateTime = DateTime.UtcNow.AddHours(-1)
            });

            // act 

            try
            {
                await _service.ConfirmAsync(confirmationToken);
            }
            catch
            {
                // ignored
            }

            // assert

            _referralHotelsRepositoryMock.Verify(o => o.UpdateAsync(
                    It.Is<ReferralHotelEncrypted>(referralHotelEncrypted =>
                        referralHotelEncrypted.State == ReferralHotelState.Expired)),
                Times.Once);
        }

        [Fact]
        public async Task Do_Not_Confirm_Referral_Hotel_If_Does_Not_Exist()
        {
            // arrange

            var confirmationToken = "token";

            // act 

            var task = _service.ConfirmAsync(confirmationToken);

            // assert

            await Assert.ThrowsAsync<ReferralDoesNotExistException>(async () => await task);
        }

        [Fact]
        public async Task Do_Not_Confirm_Referral_Hotel_If_An_Other_Confirmed_Exist()
        {
            // arrange

            var confirmationToken = "token";

            _referralHotelsEncrypted.AddRange(new[]
            {
                new ReferralHotelEncrypted
                {
                    Id = _referralId.ToString(),
                    EmailHash = "hash",
                    ConfirmationToken = confirmationToken,
                    State = ReferralHotelState.Pending
                },
                new ReferralHotelEncrypted
                {
                    Id = _referralId.ToString(),
                    EmailHash = "hash",
                    ConfirmationToken = "token1",
                    State = ReferralHotelState.Confirmed,
                    ExpirationDateTime = DateTime.UtcNow.AddHours(1)
                }
            });

            // act 

            var task = _service.ConfirmAsync(confirmationToken);

            // assert

            await Assert.ThrowsAsync<ReferralAlreadyConfirmedException>(async () => await task);
        }

        [Fact]
        public async Task Do_Not_Confirm_Referral_Hotel_If_Expired()
        {
            // arrange

            var confirmationToken = "token";

            _referralHotelsEncrypted.Add(new ReferralHotelEncrypted
            {
                Id = _referralId.ToString(),
                EmailHash = "hash",
                ConfirmationToken = confirmationToken,
                State = ReferralHotelState.Pending,
                ExpirationDateTime = DateTime.UtcNow.AddHours(-1)
            });

            // act 

            var task = _service.ConfirmAsync(confirmationToken);

            // assert

            await Assert.ThrowsAsync<ReferralExpiredException>(async () => await task);
        }

        [Fact]
        public async Task Do_Not_Use_Referral_Hotel_If_Does_Not_Exist()
        {
            // arrange

            var model = new ReferralHotelUseModel { BuyerEmail = _email };

            // act 

            var task = _service.UseAsync(model);

            // assert

            await Assert.ThrowsAsync<ReferralDoesNotExistException>(async () => await task);
        }

        [Fact]
        public async Task Do_Not_Use_Referral_Hotel_If_Does_Not_Confirmed()
        {
            // arrange

            var model = new ReferralHotelUseModel { BuyerEmail = _email };

            _referralHotelsEncrypted.Add(new ReferralHotelEncrypted
            {
                EmailHash = _email.ToSha256Hash(),
                State = ReferralHotelState.Pending
            });

            // act 

            var task = _service.UseAsync(model);

            // assert

            await Assert.ThrowsAsync<ReferralNotConfirmedException>(async () => await task);
        }

        [Fact]
        public async Task Do_Not_Use_Referral_Hotel_If_Already_Used()
        {
            // arrange

            var model = new ReferralHotelUseModel { BuyerEmail = _email };

            _referralHotelsEncrypted.Add(new ReferralHotelEncrypted
            {
                EmailHash = _email.ToSha256Hash(),
                State = ReferralHotelState.Used
            });

            // act 

            var task = _service.UseAsync(model);

            // assert

            await Assert.ThrowsAsync<ReferralAlreadyUsedException>(async () => await task);
        }

        [Fact]
        public async Task Do_Not_Use_Referral_Hotel_If_Status_Expired()
        {
            // arrange

            var model = new ReferralHotelUseModel { BuyerEmail = _email };

            _referralHotelsEncrypted.Add(new ReferralHotelEncrypted
            {
                EmailHash = _email.ToSha256Hash(),
                State = ReferralHotelState.Expired
            });

            // act 

            var task = _service.UseAsync(model);

            // assert

            await Assert.ThrowsAsync<ReferralExpiredException>(async () => await task);
        }

        [Fact]
        public async Task Do_Not_Use_Referral_Hotel_If_Expired()
        {
            // arrange

            var model = new ReferralHotelUseModel { BuyerEmail = _email };

            _referralHotelsEncrypted.Add(new ReferralHotelEncrypted
            {
                EmailHash = _email.ToSha256Hash(),
                State = ReferralHotelState.Confirmed,
                ExpirationDateTime = DateTime.UtcNow.AddHours(-1)
            });

            // act 

            var task = _service.UseAsync(model);

            // assert

            await Assert.ThrowsAsync<ReferralExpiredException>(async () => await task);
        }

        [Fact]
        public async Task Set_Referral_Hotel_Status_To_Expired_While_Using()
        {
            // arrange

            var model = new ReferralHotelUseModel { BuyerEmail = _email };

            _referralHotelsEncrypted.Add(new ReferralHotelEncrypted
            {
                EmailHash = _email.ToSha256Hash(),
                State = ReferralHotelState.Confirmed,
                ExpirationDateTime = DateTime.UtcNow.AddHours(-1)
            });

            // act 

            try
            {
                await _service.UseAsync(model);
            }
            catch
            {
                // ignored
            }

            // assert

            _referralHotelsRepositoryMock.Verify(o => o.UpdateAsync(
                    It.Is<ReferralHotelEncrypted>(referralHotelEncrypted =>
                        referralHotelEncrypted.State == ReferralHotelState.Expired)),
                Times.Once);
        }

        [Fact]
        public async Task Publish_Referral_Hotel_Used_While_Using()
        {
            // arrange

            var model = new ReferralHotelUseModel
            {
                BuyerEmail = _email,
                Location = "location",
                PartnerId = Guid.NewGuid().ToString("D"),
                Amount = 10,
                CurrencyCode = "currencyCode"
            };

            _referralHotelsEncrypted.Add(new ReferralHotelEncrypted
            {
                Id = _referralId.ToString(),
                EmailHash = _email.ToSha256Hash(),
                State = ReferralHotelState.Confirmed,
                ExpirationDateTime = DateTime.UtcNow.AddHours(1),
                PartnerId = model.PartnerId
            });

            // act 

            var referralHotel = await _service.UseAsync(model);

            // assert

            _rabbitPublisherMock.Verify(o => o.PublishAsync(
                    It.Is<HotelReferralUsedEvent>(message =>
                        message.Amount == model.Amount &&
                        message.CurrencyCode == model.CurrencyCode &&
                        message.CustomerId == referralHotel.ReferrerId &&
                        message.PartnerId == referralHotel.PartnerId &&
                        message.LocationId == referralHotel.Location)),
                Times.Once);
        }

        [Fact]
        public async Task Set_Referral_Hotel_Status_To_Used_While_Using()
        {
            // arrange

            var model = new ReferralHotelUseModel
            {
                BuyerEmail = _email,
                Location = "location",
                Amount = 10,
                CurrencyCode = "currencyCode"
            };

            _referralHotelsEncrypted.Add(new ReferralHotelEncrypted
            {
                Id = _referralId.ToString(),
                EmailHash = _email.ToSha256Hash(),
                State = ReferralHotelState.Confirmed,
                ExpirationDateTime = DateTime.UtcNow.AddHours(1)
            });

            // act 

            var referralHotel = await _service.UseAsync(model);

            // assert

            _referralHotelsRepositoryMock.Verify(o => o.UpdateAsync(
                    It.Is<ReferralHotelEncrypted>(referralHotelEncrypted =>
                        referralHotelEncrypted.State == ReferralHotelState.Used &&
                        referralHotelEncrypted.Location == model.Location &&
                        referralHotelEncrypted.PartnerId == referralHotel.PartnerId)),
                Times.Once);
        }
    }
}
