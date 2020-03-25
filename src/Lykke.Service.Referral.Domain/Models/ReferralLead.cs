using System;
using Lykke.Service.Referral.Domain.Entities;

namespace Lykke.Service.Referral.Domain.Models
{
    public class ReferralLead
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int PhoneCountryCodeId { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Note { get; set; }

        public Guid AgentId { get; set; }

        public string AgentSalesforceId { get; set; }

        public string SalesforceId { get; set; }

        public string ResponseStatus { get; set; }
        
        public string ConfirmationToken { get; set; }

        public Guid? CampaignId { get; set; }

        public ReferralLeadState State { get; set; }

        public DateTime CreationDateTime { get; set; }
    }
}
