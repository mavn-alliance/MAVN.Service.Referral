using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Common.MsSql;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using ReferralFriendState = MAVN.Service.Referral.MsSqlRepositories.Entities.ReferralFriendState;

namespace MAVN.Service.Referral.MsSqlRepositories.Repositories
{
    public class FriendReferralHistoryRepository : IFriendReferralHistoryRepository
    {
        private readonly IDbContextFactory<ReferralContext> _msSqlContextFactory;
        private readonly IMapper _mapper;

        public FriendReferralHistoryRepository(
            IDbContextFactory<ReferralContext> msSqlContextFactory,
            IMapper _mapper)
        {
            _msSqlContextFactory = msSqlContextFactory;
            this._mapper = _mapper;
        }

        public async Task<FriendReferralHistory> GetAllReferralsForCustomerAsync(Guid customerId)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var referrals = await context.FriendReferrals
                    .Where(c => c.ReferrerId == customerId)
                    .ToListAsync();
                
                return new FriendReferralHistory
                {
                    ReferrerId = customerId.ToString("D"),
                    ReferredIds = referrals
                        .Where(e => e.ReferredId.HasValue)
                        .Select(e => e.ReferredId.Value.ToString("D"))
                        .ToList()
                        .AsReadOnly()
                };
            }
        }

        public async Task<ReferralFriend> CreateAsync(ReferralFriend referral)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = _mapper.Map<FriendReferralEntity>(referral);
                await context.AddAsync(entity);

                await context.SaveChangesAsync();

                return _mapper.Map<ReferralFriend>(entity);
            }
        }

        public async Task UpdateAsync(ReferralFriend referral)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                context.Update(_mapper.Map<FriendReferralEntity>(referral));

                await context.SaveChangesAsync();
            }
        }

        public async Task<ReferralFriend> GetByIdAsync(Guid referralFriendId)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var result = await context.FriendReferrals.FirstOrDefaultAsync(r => r.Id == referralFriendId);

                return _mapper.Map<ReferralFriend>(result);
            }
        }

        public async Task<ReferralFriend> GetAcceptedAsync(Guid customerId)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var result = await context.FriendReferrals
                    .FirstOrDefaultAsync(r => r.ReferredId == customerId && r.State == ReferralFriendState.Accepted);

                return _mapper.Map<ReferralFriend>(result);
            }
        }

        public async Task<IReadOnlyList<ReferralFriend>> GetByReferrerIdAsync(
            Guid referrerId,
            Guid? campaignId, 
            IEnumerable<Domain.Models.ReferralFriendState> states)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var result = context.FriendReferrals.Where(r => r.ReferrerId == referrerId);

                if (campaignId.HasValue)
                {
                    result = result.Where(r => r.CampaignId == campaignId);
                }

                if (states != null && states.Any())
                {
                    var mappedStates = _mapper.Map<IEnumerable<ReferralFriendState>>(states);
                    result = result.Where(r => mappedStates.Contains(r.State));
                }

                return _mapper.Map<List<ReferralFriend>>(await result.ToArrayAsync());
            }
        }

        public async Task<IReadOnlyList<ReferralFriend>> GetReferralFriendsByReferralIdsAsync(List<Guid> referralIds)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var result = await context.FriendReferrals.Where(l => referralIds.Contains(l.Id)).ToListAsync();

                return _mapper.Map<List<ReferralFriend>>(result);
            }
        }
    }
}
