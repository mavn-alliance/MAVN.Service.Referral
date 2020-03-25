namespace Lykke.Service.Referral.Client.Models.Requests
{
    /// <summary>
    /// GetReferralHotelByEmail request model.
    /// </summary>
    public class GetHotelReferralsByEmailRequestModel
    {
        /// <summary>Email</summary>
        public string Email { get; set; }

        /// <summary>Partner id</summary>
        public string PartnerId { get; set; }

        /// <summary>Location id</summary>
        public string Location { get; set; }
    }
}
