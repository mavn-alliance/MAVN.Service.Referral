using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.MsSql;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.Referral.MsSqlRepositories.Repositories
{
    public class OfferToPurchasePurchaseRepository : IOfferToPurchasePurchaseRepository
    {
        private readonly MsSqlContextFactory<ReferralContext> _msSqlContextFactory;
        private readonly IMapper _mapper;

        public OfferToPurchasePurchaseRepository(MsSqlContextFactory<ReferralContext> msSqlContextFactory, IMapper mapper)
        {
            _msSqlContextFactory = msSqlContextFactory;
            _mapper = mapper;
        }

        public async Task InsertAsync(OfferToPurchase offerToPurchase)
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                await context.AddAsync(_mapper.Map<OfferToPurchaseEntity>(offerToPurchase));

                await context.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<OfferToPurchase>> GetAsync()
        {
            using (var context = _msSqlContextFactory.CreateDataContext())
            {
                var entities = await context.OfferToPurchases.ToListAsync();

                return _mapper.Map<IReadOnlyList<OfferToPurchase>>(entities);
            }
        }
    }
}
