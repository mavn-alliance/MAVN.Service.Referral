using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Referral.Domain.Models;

namespace Lykke.Service.Referral.Domain.Repositories
{
    public interface IOfferToPurchasePurchaseRepository
    {
        Task InsertAsync(OfferToPurchase propertyPurchase);
        Task<IReadOnlyList<OfferToPurchase>> GetAsync();
    }
}
