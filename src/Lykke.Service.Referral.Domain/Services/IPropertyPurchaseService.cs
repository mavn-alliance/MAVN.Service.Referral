using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Referral.Domain.Models;

namespace Lykke.Service.Referral.Domain.Services
{
    public interface IPropertyPurchaseService
    {
        Task<IReadOnlyList<PropertyPurchase>> GetPropertyPurchasesAsync();
        Task<Guid> AddRealEstatePurchase(PropertyPurchase propertyPurchase);
    }
}
