namespace Lykke.Service.Referral.Domain.Models
{
    public class ReferralHotelWithProfile : ReferralHotel
    {
        /// <summary>
        /// ID of the customer in customer profile
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// The first name of the referral lead
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the referral lead
        /// </summary>
        public string LastName { get; set; }
    }
}
