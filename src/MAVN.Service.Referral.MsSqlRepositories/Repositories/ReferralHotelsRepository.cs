using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using ReferralHotelState = MAVN.Service.Referral.MsSqlRepositories.Entities.ReferralHotelState;

namespace MAVN.Service.Referral.MsSqlRepositories.Repositories
{
    public class ReferralHotelsRepository : IReferralHotelsRepository
    {
        private readonly PostgreSQLContextFactory<ReferralContext> _msSqlContextFactory;
        private readonly IMapper _mapper;

        public ReferralHotelsRepository(
            PostgreSQLContextFactory<ReferralContext> msSqlContextFactory, IMapper mapper)
        {
            _msSqlContextFactory = msSqlContextFactory;
            _mapper = mapper;
        }

        public async Task<ReferralHotelEncrypted> CreateAsync(ReferralHotelEncrypted referralHotelEncrypted)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = _mapper.Map<ReferralHotelEntity>(referralHotelEncrypted);

                await context.AddAsync(entity);

                await context.SaveChangesAsync();

                return _mapper.Map<ReferralHotelEncrypted>(entity);
            }
        }

        public async Task<ReferralHotelEncrypted> UpdateAsync(ReferralHotelEncrypted referralHotelEncrypted)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = _mapper.Map<ReferralHotelEntity>(referralHotelEncrypted);

                context.Update(entity);

                await context.SaveChangesAsync();

                return _mapper.Map<ReferralHotelEncrypted>(entity);
            }
        }

        public async Task<IReadOnlyList<ReferralHotelEncrypted>> GetByEmailHashAsync(string emailHash, string partnerId,
            string location)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entitiesQuery = context.ReferralHotels
                    .Where(x => x.EmailHash == emailHash);

                if (!string.IsNullOrWhiteSpace(partnerId))
                {
                    entitiesQuery = entitiesQuery.Where(x => string.IsNullOrEmpty(x.PartnerId) || x.PartnerId == partnerId);
                }

                if (!string.IsNullOrWhiteSpace(location))
                {
                    entitiesQuery = entitiesQuery.Where(x => string.IsNullOrEmpty(x.PartnerId) || x.Location == location);
                }

                var entities = await entitiesQuery.ToListAsync();

                return _mapper.Map<IReadOnlyList<ReferralHotelEncrypted>>(entities);
            }
        }

        public async Task<IReadOnlyList<ReferralHotelEncrypted>> GetByReferrerIdAsync(string referrerId)
        {
            return await GetByReferrerIdAsync(referrerId, null, null);
        }

        public async Task<IReadOnlyList<ReferralHotelEncrypted>> GetByReferrerIdAsync(
            string referrerId, 
            Guid? campaignId, 
            IEnumerable<Domain.Models.ReferralHotelState> states)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entities = context.ReferralHotels
                    .Where(x => x.ReferrerId == referrerId);

                if (campaignId.HasValue)
                {
                    entities = entities.Where(c => c.CampaignId == campaignId.Value);
                }

                if (states != null && states.Any())
                {
                    var mappedStates = states.Select(s => _mapper.Map<ReferralHotelState>(s));

                    entities = entities.Where(c => mappedStates.Contains(c.State));
                }

                return _mapper.Map<IReadOnlyList<ReferralHotelEncrypted>>(await entities.ToListAsync());
            }
        }

        public async Task<ReferralHotelEncrypted> GetByConfirmationTokenAsync(string confirmationToken)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = await context.ReferralHotels
                    .Where(x => x.ConfirmationToken == confirmationToken)
                    .SingleOrDefaultAsync();

                return _mapper.Map<ReferralHotelEncrypted>(entity);
            }
        }

        public async Task<IReadOnlyList<ReferralHotelEncrypted>> GetByReferralIdsAsync(List<Guid> referralIds)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var result = await context.ReferralHotels.Where(l => referralIds.Contains(l.Id)).ToListAsync();

                return _mapper.Map<List<ReferralHotelEncrypted>>(result);
            }
        }
    }
}
