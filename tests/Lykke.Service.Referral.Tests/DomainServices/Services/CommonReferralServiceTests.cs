using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Lykke.Service.Referral.Domain.Exceptions;
using Lykke.Service.Referral.Domain.Models;
using Moq;
using Xunit;

namespace Lykke.Service.Referral.Tests.DomainServices.Services
{
    public class CommonReferralServiceTests
    {
        [Fact]
        public async Task ShouldTakeAllCommonReferralsByReferralIds_WhenValidDataIsPassed()
        {
            // Arrange
            var fixture = new CommonReferralServiceTestsFixture()
            {
                ReferralLeadList = new List<ReferralLeadWithDetails>
                {
                    new ReferralLeadWithDetails { Id = Guid.NewGuid() },
                    new ReferralLeadWithDetails { Id = Guid.NewGuid() }
                },
                ReferralHotelList = new List<ReferralHotelWithProfile>
                {
                    new ReferralHotelWithProfile { Id = Guid.NewGuid().ToString("D") },
                    new ReferralHotelWithProfile { Id = Guid.NewGuid().ToString("D") },
                    new ReferralHotelWithProfile { Id = Guid.NewGuid().ToString("D") }
                }
            };

            // Act
            var referrals = await fixture.Service.GetByReferralIdsAsync(new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            });

            Assert.Equal(5, referrals.Count);
            Assert.Equal(2, referrals.Count(r => r.Value.ReferralType == ReferralType.RealEstate));
            Assert.Equal(3, referrals.Count(r => r.Value.ReferralType == ReferralType.Hospitality));
        } 

        [Fact]
        public async Task ShouldTakeNoCommonReferralsByReferralIds_WhenValidDataIsPassed()
        {
            // Arrange
            var fixture = new CommonReferralServiceTestsFixture();

            // Act
            var referrals = await fixture.Service.GetByReferralIdsAsync(new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            });

            Assert.Equal(0, referrals.Count);
        }

        [Fact]
        public async Task ShouldTakeAllCommonReferralsByState_WhenValidDataIsPassed()
        {
            // Arrange
            var fixture = new CommonReferralServiceTestsFixture()
            {
                ReferralLeadList = new List<ReferralLeadWithDetails>
                {
                    new ReferralLeadWithDetails { Id = Guid.NewGuid() },
                    new ReferralLeadWithDetails { Id = Guid.NewGuid() }
                },
                ReferralHotelList = new List<ReferralHotelWithProfile>
                {
                    new ReferralHotelWithProfile { Id = Guid.NewGuid().ToString("D") },
                    new ReferralHotelWithProfile { Id = Guid.NewGuid().ToString("D") },
                    new ReferralHotelWithProfile { Id = Guid.NewGuid().ToString("D") }
                },
                ReferralFriendList = new List<ReferralFriend>
                {
                    new ReferralFriend { Id = Guid.NewGuid(), FullName = "Test one"}
                }
            };

            // Act
            var referrals = await fixture.Service.GetForCustomerAsync(
                Guid.NewGuid(), 
                Guid.NewGuid(),
                new List<CommonReferralStatus> { CommonReferralStatus.Accepted });

            Assert.Equal(6, referrals.Count);
            Assert.Equal(2, referrals.Count(r => r.ReferralType == ReferralType.RealEstate));
            Assert.Equal(3, referrals.Count(r => r.ReferralType == ReferralType.Hospitality));
            Assert.Equal(1, referrals.Count(r => r.ReferralType == ReferralType.Friend));
        }

        [Fact]
        public async Task ShouldTakeNoCommonReferralsByState_WhenValidDataIsPassed()
        {
            // Arrange
            var fixture = new CommonReferralServiceTestsFixture();

            // Act
            var referrals = await fixture.Service.GetForCustomerAsync(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new List<CommonReferralStatus> { CommonReferralStatus.Accepted });

            Assert.Equal(0, referrals.Count);
        }
    }
}
