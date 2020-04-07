using Autofac;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Sdk;
using MAVN.Service.Referral.Domain.Managers;
using MAVN.Service.Referral.Domain.Repositories;
using MAVN.Service.Referral.Domain.Services;
using MAVN.Service.Referral.DomainServices.Managers;
using MAVN.Service.Referral.DomainServices.Services;
using MAVN.Service.Referral.Managers;
using MAVN.Service.Referral.Settings;
using Lykke.SettingsReader;

namespace MAVN.Service.Referral.Modules
{
    [UsedImplicitly]
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(
                new MsSqlRepositories.AutofacModule(_appSettings.CurrentValue.ReferralService.Db.MsSqlConnectionString));

            builder.RegisterType<HashingManager>()
                .As<IHashingManager>()
                .SingleInstance();

            builder.RegisterType<CommissionManager>()
                .As<CommissionManager>()
                .SingleInstance();

            builder.RegisterType<NotificationPublisherService>()
                .As<INotificationPublisherService>()
                .WithParameter("hotelReferralConfirmEmailLinkFormat", _appSettings.CurrentValue.ReferralService.HotelReferralConfirmEmailSettings.ConfirmLinkFormat)
                .WithParameter("hotelReferralConfirmHotelReservationLink", _appSettings.CurrentValue.ReferralService.HotelReferralConfirmEmailSettings.HotelReservationLink)
                .WithParameter("hotelReferralConfirmEmailTemplateId", _appSettings.CurrentValue.ReferralService.HotelReferralConfirmEmailSettings.EmailTemplateId)
                .WithParameter("hotelReferralConfirmEmailSubjectTemplateId", _appSettings.CurrentValue.ReferralService.HotelReferralConfirmEmailSettings.SubjectTemplateId)
                .WithParameter("friendReferralConfirmEmailTemplateId", _appSettings.CurrentValue.ReferralService.FriendReferralConfirmEmailSettings.EmailTemplateId)
                .WithParameter("friendReferralConfirmEmailSubjectTemplateId", _appSettings.CurrentValue.ReferralService.FriendReferralConfirmEmailSettings.SubjectTemplateId)
                .WithParameter("leadConfirmSmsLinkFormat", _appSettings.CurrentValue.ReferralService.LeadConfirmSmsSettings.ConfirmLinkFormat)
                .WithParameter("leadConfirmSmsTemplateId", _appSettings.CurrentValue.ReferralService.LeadConfirmSmsSettings.SmsTemplateId)
                .WithParameter("leadAlreadyConfirmedEmailTemplateId", _appSettings.CurrentValue.ReferralService.LeadAlreadyConfirmedEmailSettings.EmailTemplateId)
                .WithParameter("leadAlreadyConfirmedEmailSubjectTemplateId", _appSettings.CurrentValue.ReferralService.LeadAlreadyConfirmedEmailSettings.SubjectTemplateId)
                .WithParameter("leadSuccessfullyConfirmedEmailTemplateId", _appSettings.CurrentValue.ReferralService.LeadSuccessfullyConfirmedEmailSettings.EmailTemplateId)
                .WithParameter("leadSuccessfullyConfirmedEmailSubjectTemplateId", _appSettings.CurrentValue.ReferralService.LeadSuccessfullyConfirmedEmailSettings.SubjectTemplateId)
                .WithParameter("leadSuccessfullyConfirmedPushNotificationTemplateId", _appSettings.CurrentValue.ReferralService.LeadSuccessfullyConfirmedPushNotificationSettings.TemplateId)
                .WithParameter("deepLinkReferralList", _appSettings.CurrentValue.ReferralService.LeadSuccessfullyConfirmedPushNotificationSettings.DeepLinkRoute)
                .WithParameter("friendConfirmLinkFormat", _appSettings.CurrentValue.ReferralService.FriendReferralConfirmEmailSettings.ConfirmLinkFormat)
                .WithParameter("useShortenedLeadUrl", _appSettings.CurrentValue.ReferralService.UseShortenedLeadUrl)
                .SingleInstance();
            
            builder.RegisterType<SettingsService>()
                .As<ISettingsService>()
                .WithParameter("leadConfirmationTokenLength", _appSettings.CurrentValue.ReferralService.LeadConfirmationTokenLength)
                .WithParameter("demoEmailIdentifier", _appSettings.CurrentValue.ReferralService.EmailDemoIdentifier)
                .SingleInstance();

            builder.RegisterType<FriendReferralService>()
                .As<IFriendReferralService>()
                .SingleInstance();

            //builder.RegisterType<ReferralLeadService>()
            //    .As<IReferralLeadService>()
            //    .SingleInstance();

            builder.RegisterType<ReferralHotelsService>()
                .As<IReferralHotelsService>()
                .WithParameter("referralExpirationPeriod", _appSettings.CurrentValue.ReferralService.ReferralHotelExpirationPeriod)
                .WithParameter("referralLimitPeriod", _appSettings.CurrentValue.ReferralService.ReferralHotelLimitPeriod)
                .WithParameter("referralLimit", _appSettings.CurrentValue.ReferralService.ReferralHotelLimit)
                .WithParameter("globalBaseCurrencyCode", _appSettings.CurrentValue.ReferralService.BaseCurrencyCode)
                .SingleInstance();

            builder.RegisterType<PropertyPurchaseService>()
                .As<IPropertyPurchaseService>()
                .SingleInstance();

            builder.RegisterType<OfferToPurchaseService>()
                .As<IOfferToPurchaseService>()
                .SingleInstance();

            builder.RegisterType<StakeService>()
                .As<IStakeService>()
                .SingleInstance();

            builder.RegisterType<CommonReferralService>()
                .As<ICommonReferralService>()
                .SingleInstance();

            builder.RegisterType<DemoLeadService>()
                .As<IDemoLeadService>()
                .WithParameter("referralExpirationPeriod", _appSettings.CurrentValue.ReferralService.ReferralHotelExpirationPeriod)
                .WithParameter("delay", _appSettings.CurrentValue.ReferralService.DemoOperationDelay)
                .SingleInstance();

            builder.RegisterType<DemoHotelService>()
                .As<IDemoHotelService>()
                .WithParameter("referralExpirationPeriod", _appSettings.CurrentValue.ReferralService.ReferralHotelExpirationPeriod)
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.ReferralService.BaseCurrencyCode))
                .SingleInstance();

            builder.Register(c =>
                {
                    var repository = c.Resolve<IReferralRepository>();
                    var hashingManager = c.Resolve<IHashingManager>();
                    var logFactory = c.Resolve<ILogFactory>();
                    return new ReferralService(
                        repository,
                        _appSettings.CurrentValue.ReferralService.ReferralCodeLength,
                        hashingManager,
                        logFactory);
                })
                .As<IReferralService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .SingleInstance()
                .AutoActivate();
        }
    }
}
