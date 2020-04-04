using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAVN.Service.Referral.MsSqlRepositories.Entities
{
    [Table("referral_hotel")]
    public class ReferralHotelEntity : BaseEntity
    {
        [Column("email_hash", TypeName = "char(64)")]
        public string EmailHash { get; set; }
        
        [Column("name_hash", TypeName = "char(64)")]
        public string FullNameHash { get; set; }
        
        [Column("phone_number_hash", TypeName = "char(64)")]
        public string PhoneNumberHash { get; set; }
        
        [Column("phone_country_code_id")]
        public int PhoneCountryCodeId { get; set; }

        [Column("referrer_id")]
        public string ReferrerId { get; set; }
        
        [Column("confirmation_token")]
        public string ConfirmationToken { set; get; }
        
        [Column("partner_id")]
        public string PartnerId { set; get; }
        
        [Column("location")]
        public string Location { set; get; }

        [Column("campaign_id")]
        public Guid? CampaignId { get; set; }

        [Column("stake_enabled")]
        public bool StakeEnabled { get; set; }

        [Column("state")]
        public ReferralHotelState State { get; set; }

        [Column("creation_datetime")]
        public DateTime CreationDateTime { get; set; }

        [Column("expiration_datetime")]
        public DateTime ExpirationDateTime { get; set; }
    }
}
