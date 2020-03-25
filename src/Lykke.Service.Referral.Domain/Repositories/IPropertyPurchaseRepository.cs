using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Referral.Domain.Models;

namespace Lykke.Service.Referral.Domain.Repositories
{
    public interface IPropertyPurchaseRepository
    {
        Task InsertAsync(PropertyPurchase propertyPurchase);
        Task<IReadOnlyList<PropertyPurchase>> GetAsync();
        Task<int> GetUniqueLeadCount();
        Task<bool> PropertyPurchaseExistsAsync(Guid referralLeadId);
    }
}
