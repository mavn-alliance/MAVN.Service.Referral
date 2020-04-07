using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Service.Referral.Domain.Entities;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Services;

namespace MAVN.Service.Referral.DomainServices.Services
{
    public class CommonReferralService: ICommonReferralService
    {
        //private readonly IReferralLeadService _referralLeadService;
        private readonly IReferralHotelsService _referralHotelsService;
        private readonly IFriendReferralService _friendReferralService;
        private readonly IMapper _mapper;

        public CommonReferralService(
            //IReferralLeadService referralLeadService,
            IReferralHotelsService referralHotelsService,
            IFriendReferralService friendReferralService,
            IMapper mapper)
        {
            //_referralLeadService = referralLeadService;
            _referralHotelsService = referralHotelsService;
            _friendReferralService = friendReferralService;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<CommonReferralModel>> GetForCustomerAsync(
            Guid customerId,
            Guid? campaignId,
            List<CommonReferralStatus> statuses)
        {
            var commonReferrals = new List<CommonReferralModel>();

            //var referralLeads = await _referralLeadService.GetReferralLeadsForReferrerAsync(
            //    customerId,
            //    campaignId,
            //    _mapper.Map<List<ReferralLeadState>>(statuses));
            //if (referralLeads.Any())
            //{
            //    commonReferrals.AddRange(_mapper.Map<List<CommonReferralModel>>(referralLeads));
            //}

            var referralHotels = await _referralHotelsService.GetByReferrerIdAsync(
                customerId.ToString("D"),
                campaignId,
                _mapper.Map<List<ReferralHotelState>>(statuses));
            if (referralHotels.Any())
            {
                commonReferrals.AddRange(_mapper.Map<List<CommonReferralModel>>(referralHotels));
            }

            // The friend referral does not have expired
            var referralFriendStates = _mapper.Map<IEnumerable<ReferralFriendState>>(statuses.Where(s => s != CommonReferralStatus.Expired));
            if ((statuses.Any() && referralFriendStates.Any()) || statuses.Count == 0)
            {
                var referralFriends = await _friendReferralService.GetByReferrerIdAsync(
                    customerId,
                    campaignId,
                    referralFriendStates);

                if (referralFriends.Any())
                {
                    commonReferrals.AddRange(_mapper.Map<List<CommonReferralModel>>(referralFriends));
                }
            }


            return commonReferrals.OrderByDescending(c => c.TimeStamp).ToList();
        }

        public async Task<IDictionary<string, CommonReferralModel>> GetByReferralIdsAsync(List<Guid> referralIds)
        {
            var commonReferrals = new List<CommonReferralModel>();

            //var referralLeads = await _referralLeadService.GetReferralLeadsByReferralIdsAsync(referralIds);
            //if (referralLeads.Any())
            //{
            //    commonReferrals.AddRange(_mapper.Map<List<CommonReferralModel>>(referralLeads));
            //}

            var referralHotels = await _referralHotelsService.GetReferralHotelsByReferralIdsAsync(referralIds);
            if (referralHotels.Any())
            {
                commonReferrals.AddRange(_mapper.Map<List<CommonReferralModel>>(referralHotels));
            }

            var referralFriends = await _friendReferralService.GetReferralFriendsByReferralIdsAsync(referralIds);
            if (referralFriends.Any())
            {
                commonReferrals.AddRange(_mapper.Map<List<CommonReferralModel>>(referralFriends));
            }

            return commonReferrals.ToDictionary(k => k.Id, v => v);
        }
    }
}
