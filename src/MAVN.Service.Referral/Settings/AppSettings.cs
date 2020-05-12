using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using MAVN.Service.Campaign.Client;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.Dictionaries.Client;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.Staking.Client;
using Lykke.Service.UrlShortener.Client;

namespace MAVN.Service.Referral.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public ReferralSettings ReferralService { get; set; }

        
        public CustomerProfileServiceClientSettings CustomerProfileServiceClient { get; set; }
        
        public CurrencyConvertorServiceClientSettings CurrencyConvertorServiceClient { get; set; }
        
        public DictionariesServiceClientSettings DictionariesServiceClient { get; set; }

        public CampaignServiceClientSettings CampaignServiceClient { get; set; }

        public StakingServiceClientSettings StakingServiceClient { get; set; }

        public PartnerManagementServiceClientSettings PartnerManagementServiceClient { get; set; }
        
        public UrlShortenerServiceClientSettings UrlShortenerServiceClient { get; set; }
    }
}
