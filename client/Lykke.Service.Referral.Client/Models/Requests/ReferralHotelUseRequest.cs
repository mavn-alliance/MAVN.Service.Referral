namespace Lykke.Service.Referral.Client.Models.Requests
{
    /// <summary>
    /// Represents a request to use a hotel referral
    /// </summary>
    public class ReferralHotelUseRequest
    {
        /// <summary>
        /// Buyer's email
        /// </summary>
        public string BuyerEmail { set; get; }
        
        /// <summary>
        /// Partner Id
        /// </summary>
        public string PartnerId { set; get; }
        
        /// <summary>
        /// Location
        /// </summary>
        public string Location { set; get; }
        
        /// <summary>
        /// Amount spent
        /// </summary>
        public decimal Amount { set; get; }
        
        /// <summary>
        /// Currency
        /// </summary>
        public string CurrencyCode { set; get; }
    }
}