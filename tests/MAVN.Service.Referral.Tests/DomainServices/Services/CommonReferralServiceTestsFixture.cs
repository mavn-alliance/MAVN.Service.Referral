﻿using System;
using System.Collections.Generic;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Services;
using MAVN.Service.Referral.DomainServices.Services;
using Moq;

namespace MAVN.Service.Referral.Tests.DomainServices.Services
{
    public class CommonReferralServiceTestsFixture
    {
        public CommonReferralServiceTestsFixture()
        {
            //ReferralLeadServiceMock = new Mock<IReferralLeadService>(MockBehavior.Strict);
            ReferralHotelsServiceMock = new Mock<IReferralHotelsService>(MockBehavior.Strict);
            FriendReferralServiceMock = new Mock<IFriendReferralService>(MockBehavior.Strict);

            Service = new CommonReferralService(
                //ReferralLeadServiceMock.Object,
                ReferralHotelsServiceMock.Object,
                FriendReferralServiceMock.Object,
                MapperHelper.CreateAutoMapper());

            ReferralHotelList = new List<ReferralHotelWithProfile>();
            ReferralFriendList = new List<ReferralFriend>();

            SetupCalls();
        }

        public Mock<IFriendReferralService> FriendReferralServiceMock { get; set; }
        //public Mock<IReferralLeadService> ReferralLeadServiceMock;
        public Mock<IReferralHotelsService> ReferralHotelsServiceMock;
        public CommonReferralService Service;
        public List<ReferralHotelWithProfile> ReferralHotelList;
        public List<ReferralFriend> ReferralFriendList { get; set; }

        public void SetupCalls()
        {
            ReferralHotelsServiceMock.Setup(c => c.GetByReferrerIdAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<IEnumerable<ReferralHotelState>>()))
                .ReturnsAsync(() => ReferralHotelList);

            ReferralHotelsServiceMock.Setup(c => c.GetReferralHotelsByReferralIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(() => ReferralHotelList);

            FriendReferralServiceMock.Setup(c => c.GetByReferrerIdAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<IEnumerable<ReferralFriendState>>()))
                .ReturnsAsync(() => ReferralFriendList);

            FriendReferralServiceMock.Setup(c => c.GetReferralFriendsByReferralIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(() => ReferralFriendList);
        }

    }
}
