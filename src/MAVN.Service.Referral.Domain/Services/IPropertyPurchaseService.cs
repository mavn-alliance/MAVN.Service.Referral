using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.Domain.Services
{
    public interface IPropertyPurchaseService
    {
        Task<IReadOnlyList<PropertyPurchase>> GetPropertyPurchasesAsync();
        Task<Guid> AddRealEstatePurchase(PropertyPurchase propertyPurchase);
    }
}
