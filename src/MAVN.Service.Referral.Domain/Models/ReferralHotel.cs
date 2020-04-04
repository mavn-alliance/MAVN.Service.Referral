using System;

namespace MAVN.Service.Referral.Domain.Models
{
    public class ReferralHotel
    {
        public string Id { get; set; }
        
        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public int PhoneCountryCodeId { get; set; }
        
        public string FullName { get; set; }
        
        public string ReferrerId { get; set; }
        
        public string Location { set; get; }
        
        public string PartnerId { set; get; }
        
        public string ConfirmationToken { get; set; }
        
        public ReferralHotelState State { get; set; }

        public DateTime CreationDateTime { get; set; }

        public DateTime ExpirationDateTime { get; set; }

        public Guid? CampaignId { get; set; }
    }
}
