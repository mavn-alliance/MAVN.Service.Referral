using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.MsSql;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.Referral.MsSqlRepositories.Repositories
{
    public class PropertyPurchaseRepository : IPropertyPurchaseRepository
    {
        private readonly IDbContextFactory<ReferralContext> _msSqlContextFactory;
        private readonly IMapper _mapper;

        public PropertyPurchaseRepository(IDbContextFactory<ReferralContext> msSqlContextFactory, IMapper mapper)
        {
            _msSqlContextFactory = msSqlContextFactory;
            _mapper = mapper;
        }

        public async Task InsertAsync(PropertyPurchase propertyPurchase)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                await context.AddAsync(_mapper.Map<PropertyPurchaseEntity>(propertyPurchase));

                await context.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<PropertyPurchase>> GetAsync()
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entities = await context.PropertyPurchases.ToListAsync();

                return _mapper.Map<IReadOnlyList<PropertyPurchase>>(entities);
            }
        }

        public async Task<int> GetUniqueLeadCount()
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                // Order by throws an EF "cannot be evaluated exception" 
                var count = await context.PropertyPurchases.Select(p => p.ReferralLeadId)
                    .Distinct()
                    .CountAsync();

                return count;
            }
        }

        public async Task<bool> PropertyPurchaseExistsAsync(Guid referralLeadId)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                return await context.PropertyPurchases.AnyAsync(e => e.ReferralLeadId == referralLeadId);
            }
        }
    }
}
