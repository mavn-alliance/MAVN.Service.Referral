using System;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Common.MsSql;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.Referral.MsSqlRepositories.Repositories
{
    public class ReferralRepository: IReferralRepository
    {
        private readonly IDbContextFactory<ReferralContext> _msSqlContextFactory;
        private readonly IMapper _mapper;

        public ReferralRepository(IDbContextFactory<ReferralContext> msSqlContextFactory, IMapper mapper)
        {
            _msSqlContextFactory = msSqlContextFactory;
            _mapper = mapper;
        }

        public async Task<Domain.Models.Referral> GetByCustomerIdAsync(Guid customerId)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = await context.Referrals
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);
                
                return _mapper.Map<Domain.Models.Referral>(entity);
            }
        }

        public async Task<Domain.Models.Referral> GetByReferralCodeAsync(string referralCode)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entity = await context.Referrals
                    .FirstOrDefaultAsync(c => c.ReferralCode == referralCode);

                return _mapper.Map<Domain.Models.Referral>(entity);
            }
        }

        public async Task<bool> CreateIfNotExistAsync(Domain.Models.Referral referral)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                if (await context.Referrals.AnyAsync(x => x.CustomerId == referral.CustomerId))
                {
                    return false;
                }

                await context.AddAsync(_mapper.Map<ReferralEntity>(referral));

                await context.SaveChangesAsync();

                return true;
            }
        }
    }
}
