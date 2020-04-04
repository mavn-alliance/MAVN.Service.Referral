using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.Referral.Contract.Events;
using MAVN.Service.Referral.Domain.Models;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.Domain.Services;

namespace MAVN.Service.Referral.DomainServices.Services
{
    public class OfferToPurchaseService : IOfferToPurchaseService
    {
        private readonly IReferralLeadRepository _referralLeadRepository;
        private readonly IOfferToPurchasePurchaseRepository _offerToPurchasePurchaseRepository;
        private readonly IRabbitPublisher<OfferToPurchaseByLeadEvent> _offerToPurchasePublisher;

        public OfferToPurchaseService(
            IReferralLeadRepository referralLeadRepository,
            IOfferToPurchasePurchaseRepository offerToPurchasePurchaseRepository,
            IRabbitPublisher<OfferToPurchaseByLeadEvent> offerToPurchasePublisher)
        {
            _referralLeadRepository = referralLeadRepository;
            _offerToPurchasePurchaseRepository = offerToPurchasePurchaseRepository;
            _offerToPurchasePublisher = offerToPurchasePublisher;
        }

        public async Task<(bool isSuccessful, string errorMessage)> ProcessOfferToPurchaseAsync(
            OfferToPurchase offerToPurchase)
        {
            if (!await _referralLeadRepository.DoesExistAsync(offerToPurchase.ReferId))
            {
                return (false, $"Referral lead with id '{offerToPurchase.ReferId} does not exist.'");
            }

            await _offerToPurchasePurchaseRepository.InsertAsync(offerToPurchase);

            var lead = await _referralLeadRepository.GetAsync(offerToPurchase.ReferId);

            await _offerToPurchasePublisher.PublishAsync(new OfferToPurchaseByLeadEvent
            {
                AgentId = lead.AgentId.ToString(),
                TimeStamp = offerToPurchase.Timestamp,
                CurrencyCode = offerToPurchase.CurrencyCode, VatAmount = offerToPurchase.VatAmount,
                DiscountAmount = offerToPurchase.DiscountAmount, NetPropertyPrice = offerToPurchase.NetPropertyPrice,
                SellingPropertyPrice = offerToPurchase.SellingPropertyPrice,
                CampaignId = lead.CampaignId.Value,
                UnitLocationCode = offerToPurchase.UnitLocationCode,
                ReferralId = lead.Id.ToString()
            });

            return (true, string.Empty);
        }

        public async Task<IReadOnlyList<OfferToPurchase>> GetOffersToPurchasesAsync()
        {
            return await _offerToPurchasePurchaseRepository.GetAsync();
        }
    }
}
