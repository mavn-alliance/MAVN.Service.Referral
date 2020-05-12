using Autofac;
using JetBrains.Annotations;
using MAVN.Service.Campaign.Client;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.Dictionaries.Client;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.Referral.Settings;
using MAVN.Service.Staking.Client;
using Lykke.Service.UrlShortener.Client;
using Lykke.SettingsReader;

namespace MAVN.Service.Referral.Modules
{
    [UsedImplicitly]
    public class ClientsModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ClientsModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterCustomerProfileClient(_appSettings.CurrentValue.CustomerProfileServiceClient, null);

            builder.RegisterCurrencyConvertorClient(_appSettings.CurrentValue.CurrencyConvertorServiceClient, null);


            builder.RegisterDictionariesClient(_appSettings.CurrentValue.DictionariesServiceClient);

            builder.RegisterCampaignClient(_appSettings.CurrentValue.CampaignServiceClient);

            builder.RegisterStakingClient(_appSettings.CurrentValue.StakingServiceClient, null);

            builder.RegisterPartnerManagementClient(_appSettings.CurrentValue.PartnerManagementServiceClient, null);

            builder.RegisterUrlShortenerClient(_appSettings.CurrentValue.UrlShortenerServiceClient, null);
        }
    }
}
