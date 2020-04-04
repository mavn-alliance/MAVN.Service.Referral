using Autofac;
using JetBrains.Annotations;
using Lykke.Service.AgentManagement.Client;
using Lykke.Service.Campaign.Client;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.Dictionaries.Client;
using Lykke.Service.MAVNPropertyIntegration.Client;
using Lykke.Service.PartnerManagement.Client;
using MAVN.Service.Referral.Settings;
using Lykke.Service.Staking.Client;
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
            builder.RegisterMAVNPropertyIntegrationClient(
                _appSettings.CurrentValue.PropertyIntegrationServiceClientSettings, null);

            builder.RegisterCustomerProfileClient(_appSettings.CurrentValue.CustomerProfileServiceClient, null);

            builder.RegisterCurrencyConvertorClient(_appSettings.CurrentValue.CurrencyConvertorServiceClient, null);

            builder.RegisterAgentManagementClient(_appSettings.CurrentValue.AgentManagementServiceClient);

            builder.RegisterDictionariesClient(_appSettings.CurrentValue.DictionariesServiceClient);

            builder.RegisterCampaignClient(_appSettings.CurrentValue.CampaignServiceClient);

            builder.RegisterStakingClient(_appSettings.CurrentValue.StakingServiceClient, null);

            builder.RegisterPartnerManagementClient(_appSettings.CurrentValue.PartnerManagementServiceClient, null);

            builder.RegisterUrlShortenerClient(_appSettings.CurrentValue.UrlShortenerServiceClient, null);
        }
    }
}
