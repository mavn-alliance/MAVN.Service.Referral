using System;

namespace MAVN.Service.Referral.Domain.Exceptions
{
    public class CampaignNotFoundException : Exception
    {
        public CampaignNotFoundException()
        {
            
        }
        
        public CampaignNotFoundException(string message)
        : base(message)
        {

        }
    }
}
