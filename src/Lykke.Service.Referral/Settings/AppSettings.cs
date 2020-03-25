using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.AgentManagement.Client;
using Lykke.Service.Campaign.Client;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.Dictionaries.Client;
using Lykke.Service.MAVNPropertyIntegration.Client;
using Lykke.Service.PartnerManagement.Client;
using Lykke.Service.Staking.Client;
using Lykke.Service.UrlShortener.Client;

namespace Lykke.Service.Referral.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public ReferralSettings ReferralService { get; set; }

        public MAVNPropertyIntegrationServiceClientSettings PropertyIntegrationServiceClientSettings { get; set; }
        
        public CustomerProfileServiceClientSettings CustomerProfileServiceClient { get; set; }
        
        public CurrencyConvertorServiceClientSettings CurrencyConvertorServiceClient { get; set; }
        
        public AgentManagementServiceClientSettings AgentManagementServiceClient { get; set; }

        public DictionariesServiceClientSettings DictionariesServiceClient { get; set; }

        public CampaignServiceClientSettings CampaignServiceClient { get; set; }

        public StakingServiceClientSettings StakingServiceClient { get; set; }

        public PartnerManagementServiceClientSettings PartnerManagementServiceClient { get; set; }
        
        public UrlShortenerServiceClientSettings UrlShortenerServiceClient { get; set; }
    }
}
