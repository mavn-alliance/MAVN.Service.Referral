using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.Domain.Repositories
{
    public interface IPropertyPurchaseRepository
    {
        Task InsertAsync(PropertyPurchase propertyPurchase);
        Task<IReadOnlyList<PropertyPurchase>> GetAsync();
        Task<int> GetUniqueLeadCount();
        Task<bool> PropertyPurchaseExistsAsync(Guid referralLeadId);
    }
}
