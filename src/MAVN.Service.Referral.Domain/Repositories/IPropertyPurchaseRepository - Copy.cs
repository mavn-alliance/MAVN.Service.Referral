using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.Domain.Repositories
{
    public interface IOfferToPurchasePurchaseRepository
    {
        Task InsertAsync(OfferToPurchase propertyPurchase);
        Task<IReadOnlyList<OfferToPurchase>> GetAsync();
    }
}
