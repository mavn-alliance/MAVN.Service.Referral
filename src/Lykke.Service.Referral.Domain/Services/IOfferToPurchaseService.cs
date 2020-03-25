using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Referral.Domain.Models;

namespace Lykke.Service.Referral.Domain.Services
{
    public interface IOfferToPurchaseService
    {
        Task<(bool isSuccessful, string errorMessage)> ProcessOfferToPurchaseAsync(OfferToPurchase offerToPurchase);
        Task<IReadOnlyList<OfferToPurchase>> GetOffersToPurchasesAsync();
    }
}
