using System;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.MAVNPropertyIntegration.Contract.MAVNEvents;
using Lykke.Service.MAVNPropertyIntegration.Contract.Enums;
using Lykke.Service.Referral.Domain.Exceptions;
using Lykke.Service.Referral.Domain.Services;

namespace Lykke.Service.Referral.DomainServices.Subscribers
{
    public class PropertyLeadApprovedSubscriber : RabbitSubscriber<MAVNPropertyLeadApprovedEvent>
    {
        private readonly IReferralLeadService _referralLeadService;
        
        public PropertyLeadApprovedSubscriber(
            string connectionString, 
            string exchangeName,
            IReferralLeadService referralLeadService,
            ILogFactory logFactory) 
            : base(connectionString, exchangeName, logFactory)
        {
            GuidsFieldsToValidate.Add(nameof(MAVNPropertyLeadApprovedEvent.ReferId));
            
            _referralLeadService = referralLeadService;
        }

        public override async Task<(bool isSuccessful, string errorMessage)> ProcessMessageAsync(MAVNPropertyLeadApprovedEvent message)
        {
            // Lead was rejected
            if (message.LeadStatus == LeadStatus.ClosedDisqualified)
            {
                try
                {
                    await _referralLeadService.RejectReferralLeadAsync(Guid.Parse(message.ReferId), message.Timestamp);
                    
                    return (true, null);
                }
                catch (ReferralDoesNotExistException e)
                {
                    return (false, e.Message);
                }
                catch (InvalidOperationException e)
                {
                    return (false, e.Message);
                }
            }
            
            // Only leads in ClosedConverted are 'approved' leads in salesforce
            if (message.LeadStatus != LeadStatus.ClosedConverted)
            {
                return (true, "Lead is not in status 'ClosedConverted'");
            }

            if (string.IsNullOrWhiteSpace(message.SalesforceId))
            {
                return (false, "SalesforceId is missing in Property Lead Approved Event");
            }

            try
            {
                await _referralLeadService.ApproveReferralLeadAsync(Guid.Parse(message.ReferId), message.Timestamp);
            }
            catch (ReferralDoesNotExistException e)
            {
                return (false, e.Message);
            }
            catch (InvalidOperationException e)
            {
                return (false, e.Message);
            }

            return (true, null);
        }
    }
}
