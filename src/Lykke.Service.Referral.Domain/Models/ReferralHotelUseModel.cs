namespace Lykke.Service.Referral.Domain.Models
{
    public class ReferralHotelUseModel
    {
        public string BuyerEmail { set; get; }
        
        public string PartnerId { set; get; }
        
        public string Location { set; get; }
        
        public decimal Amount { set; get; }
        
        public string CurrencyCode { set; get; }
    }
}