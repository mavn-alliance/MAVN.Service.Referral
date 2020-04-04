using System;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.MAVNPropertyIntegration.Contract.MAVNEvents;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Services;

namespace MAVN.Service.Referral.DomainServices.Subscribers
{
    public class OfferToPurchaseByLeadSubscriber : RabbitSubscriber<MAVNOfferToPurchaseByLeadEvent>
    {
        private readonly IOfferToPurchaseService _offerToPurchaseService;

        public OfferToPurchaseByLeadSubscriber(
            string connectionString,
            string exchangeName,
            IOfferToPurchaseService offerToPurchaseService,
            ILogFactory logFactory)
            : base(connectionString, exchangeName, logFactory)
        {
            _offerToPurchaseService = offerToPurchaseService;

            GuidsFieldsToValidate.Add(nameof(MAVNOfferToPurchaseByLeadEvent.ReferId));
        }

        public override async Task<(bool isSuccessful, string errorMessage)> ProcessMessageAsync(MAVNOfferToPurchaseByLeadEvent message)
        {
            return await _offerToPurchaseService.ProcessOfferToPurchaseAsync(new OfferToPurchase
            {
                ReferId = Guid.Parse(message.ReferId),
                Timestamp = message.Timestamp,
                UnitLocationCode = message.UnitLocationCode,
                VatAmount = message.VatAmount,
                SellingPropertyPrice = message.SellingPropertyPrice,
                NetPropertyPrice = message.NetPropertyPrice,
                DiscountAmount = message.DiscountAmount,
                CurrencyCode = "AED" // Supposedly coming as AED in the event 
            });
        }
    }
}
