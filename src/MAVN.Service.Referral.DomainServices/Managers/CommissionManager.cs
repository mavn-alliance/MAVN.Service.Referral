using AutoMapper;
using MAVN.Service.Referral.Contract.Events;
using MAVN.Service.Referral.Domain.Models;

namespace MAVN.Service.Referral.DomainServices.Managers
{
    public class CommissionManager

    {
        private readonly IMapper _mapper;

        public CommissionManager(IMapper mapper)
        {
            _mapper = mapper;
        }

        public T ToCommissionEvent<T>(PropertyPurchase propertyPurchase, ReferralLeadEncrypted lead)
            where T : PropertyPurchaseReferralEvent
        {
            var propertyPurchaseEvent = new PropertyPurchaseReferralEvent
            {
                ReferrerId = lead.AgentId.ToString(),
                TimeStamp = propertyPurchase.Timestamp,
                CurrencyCode = propertyPurchase.CurrencyCode,
                VatAmount = propertyPurchase.VatAmount,
                DiscountAmount = propertyPurchase.DiscountAmount,
                NetPropertyPrice = propertyPurchase.NetPropertyPrice,
                SellingPropertyPrice = propertyPurchase.SellingPropertyPrice,
                CalculatedCommissionAmount = propertyPurchase.CalculatedCommissionAmount,
                ReferralId = lead.Id.ToString()
            };
            return _mapper.Map<T>(propertyPurchaseEvent);
        }
    }
}
