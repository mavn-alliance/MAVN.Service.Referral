using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAVN.Service.Referral.MsSqlRepositories.Entities
{
    [Table("referral_lead")]
    public class ReferralLeadEntity : BaseEntity
    {
        [Column("phone_country_code_id")]
        public int PhoneCountryCodeId { get; set; }

        [Column("phone_number_hash", TypeName = "char(64)")]
        public string PhoneNumberHash { get; set; }

        [Column("email_hash", TypeName = "char(64)")]
        public string EmailHash { get; set; }

        [Column("agent_id")]
        public Guid AgentId { get; set; }

        [Column("salesforce_id", TypeName = "varchar(200)")]
        public string SalesforceId { get; set; }

        [Column("agent_salesforce_id", TypeName = "varchar(200)")]
        public string AgentSalesforceId { get; set; }

        [Column("response_status", TypeName = "varchar(64)")]
        public string ResponseStatus { get; set; }

        [Column("confirmation_token", TypeName = "varchar(200)")]
        public string ConfirmationToken { set; get; }

        [Column("campaign_id")]
        public Guid? CampaignId { get; set; }

        [Column("stake_enabled")]
        public bool StakeEnabled { get; set; }

        [Column("state")]
        public ReferralLeadState State { get; set; }

        [Column("creation_datetime")]
        public DateTime CreationDateTime { get; set; }

        public ICollection<OfferToPurchaseEntity> OffersToPurchase { get; set; }

        public ICollection<PropertyPurchaseEntity> PropertyPurchases { get; set; }
    }
}
