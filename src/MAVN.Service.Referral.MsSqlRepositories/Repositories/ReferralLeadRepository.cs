using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.MsSql;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.Referral.MsSqlRepositories.Repositories
{
    public class ReferralLeadRepository : IReferralLeadRepository
    {
        private readonly IDbContextFactory<ReferralContext> _msSqlContextFactory;
        private readonly IMapper _mapper;

        public ReferralLeadRepository(IDbContextFactory<ReferralContext> msSqlContextFactory, IMapper mapper)
        {
            _msSqlContextFactory = msSqlContextFactory;
            _mapper = mapper;
        }

        public async Task<ReferralLeadEncrypted> CreateAsync(ReferralLeadEncrypted referralLeadEncrypted)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = _mapper.Map<ReferralLeadEntity>(referralLeadEncrypted);

                entity.CreationDateTime = DateTime.UtcNow;

                await context.AddAsync(entity);

                await context.SaveChangesAsync();

                return _mapper.Map<ReferralLeadEncrypted>(entity);
            }
        }

        public async Task<ReferralLeadEncrypted> UpdateAsync(ReferralLeadEncrypted referralLeadEncrypted)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = _mapper.Map<ReferralLeadEntity>(referralLeadEncrypted);

                context.Update(entity);

                await context.SaveChangesAsync();

                return _mapper.Map<ReferralLeadEncrypted>(entity);
            }
        }

        public async Task<IReadOnlyList<ReferralLeadEncrypted>> GetByEmailHashAsync(string emailHash)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entities = await context.ReferralLeads
                    .Where(c => c.EmailHash == emailHash)
                    .ToListAsync();

                return _mapper.Map<IReadOnlyList<ReferralLeadEncrypted>>(entities);
            }
        }

        public async Task<IReadOnlyList<ReferralLeadEncrypted>> GetByPhoneNumberHashAsync(int countryCodeId,
            string phoneNumberHash)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entities = await context.ReferralLeads
                    .Where(c => c.PhoneCountryCodeId == countryCodeId && c.PhoneNumberHash == phoneNumberHash)
                    .ToListAsync();

                return _mapper.Map<IReadOnlyList<ReferralLeadEncrypted>>(entities);
            }
        }

        public async Task<ReferralLeadEncrypted> GetByConfirmationTokenAsync(string confirmationToken)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = await context.ReferralLeads
                    .FirstOrDefaultAsync(c => c.ConfirmationToken == confirmationToken);

                return _mapper.Map<ReferralLeadEncrypted>(entity);
            }
        }

        public async Task<IReadOnlyList<ReferralLeadEncryptedWithDetails>> GetForReferrerAsync(
            Guid referrerId,
            Guid? campaignId,
            IEnumerable<Domain.Entities.ReferralLeadState> states)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entities = context.ReferralLeads
                    .Where(c => c.AgentId == referrerId);

                if (campaignId.HasValue)
                {
                    entities = entities
                        .Where(c => c.CampaignId == campaignId.Value);
                }

                if (states != null && states.Any())
                {
                    var mappedStates = states.Select(s => _mapper.Map<ReferralLeadState>(s));

                    entities = entities
                        .Where(c => mappedStates.Contains(c.State));
                }

                entities = entities.Include(o => o.OffersToPurchase)
                    .Include(o => o.PropertyPurchases);

                return _mapper.Map<IReadOnlyList<ReferralLeadEncryptedWithDetails>>(await entities.ToListAsync());
            }
        }

        public async Task<IReadOnlyList<ReferralLeadEncrypted>> GetApprovedAsync()
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entities = await context.ReferralLeads
                    .Where(c => c.State == ReferralLeadState.Approved)
                    .ToListAsync();

                return _mapper.Map<IReadOnlyList<ReferralLeadEncrypted>>(entities);
            }
        }

        public async Task<ReferralLeadEncrypted> GetAsync(Guid referLeadId)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = await context.ReferralLeads.FirstOrDefaultAsync(c => c.Id == referLeadId);

                return _mapper.Map<ReferralLeadEncrypted>(entity);
            }
        }

        public async Task<bool> DoesExistAsync(Guid referLeadId)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                return await context.ReferralLeads.AnyAsync(c => c.Id == referLeadId);
            }
        }

        public async Task<int> GetCountAsync(Domain.Entities.ReferralLeadState? status = null)
        {
            Expression<Func<ReferralLeadEntity, bool>> filter = c => true;

            if (status.HasValue)
            {
                var state = _mapper.Map<ReferralLeadState>(status);

                filter = c => c.State == state;
            }

            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var count = await context.ReferralLeads
                    .CountAsync(filter);

                return count;
            }
        }

        public async Task<Dictionary<Guid, int>> GetApprovedReferralsCountByAgentsAsync(List<Guid> agentIds)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var query = from b in context.ReferralLeads
                    where b.State == ReferralLeadState.Approved && agentIds.Contains(b.AgentId)
                    group b.Id by b.AgentId into g
                    select new Tuple<Guid, int>(g.Key, g.Count());

                var result = await query.ToListAsync();
                
                return agentIds.ToDictionary(x => x, x => result.SingleOrDefault(y => y.Item1 == x)?.Item2 ?? 0);
            }
        }

        public async Task<IReadOnlyList<ReferralLeadEncryptedWithDetails>> GetByReferralIdsAsync(List<Guid> referralIds)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var result = await context.ReferralLeads.Where(l => referralIds.Contains(l.Id)).ToListAsync();

                return _mapper.Map<List<ReferralLeadEncryptedWithDetails>>(result);
            }
        }
    }
}
