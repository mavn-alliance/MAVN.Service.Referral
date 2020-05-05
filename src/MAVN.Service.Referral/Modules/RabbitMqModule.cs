using Autofac;
using JetBrains.Annotations;
using Lykke.Common;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.NotificationSystem.SubscriberContract;
using MAVN.Service.Referral.Contract.Events;
using MAVN.Service.Referral.DomainServices.Subscribers;
using MAVN.Service.Referral.Settings;
using Lykke.SettingsReader;

namespace MAVN.Service.Referral.Modules
{
    [UsedImplicitly]
    public class RabbitMqModule : Module
    {
        // Publishers
        private const string PropertyLeadApprovedReferralExchangeName = "lykke.bonus.propertyleadapprovedreferral";
        private const string OfferToPurchaseByLeadExchangeName = "lykke.bonus.purchasereferral.offertopurchasebylead";

        private const string FriendReferredExchangeName = "lykke.bonus.friendreferral";
        private const string HotelReferralUsedExchangeName = "lykke.bonus.hotelrferral.referralused";
        private const string LeadStateChangedExchangeName = "lykke.bonus.leadstatechanged";

        // Subscribers
        private const string PropertyLeadApprovedExchangeName = "lykke.mavn.propertyintegration.propertyleadapproved";
        private const string OfferToPurchasePropertyByLeadExchangeName = "lykke.mavn.propertyintegration.offertopurchasebylead";

        private const string UserRegisteredExchangeName = "lykke.customer.registration";
        private const string NotificationSystemEmailExchangeName = "lykke.notificationsystem.command.emailmessage";
        private const string NotificationSystemSmsExchangeName = "lykke.notificationsystem.command.sms";
        private const string NotificationSystemPushNotificationExchangeName = "lykke.notificationsystem.command.pushnotification";
        private const string EmailVerifiedExchangeName = "lykke.customer.emailverified";

        private readonly string _connString;
        private readonly bool _isRealEstateFeatureDisabled;

        public RabbitMqModule(IReloadingManager<AppSettings> settingsManager)
        {
            var appSettings = settingsManager.CurrentValue;
            _connString = appSettings.ReferralService.RabbitMq.RabbitMqConnectionString;
            _isRealEstateFeatureDisabled = appSettings.ReferralService.IsRealEstateFeatureDisabled
                ?? false;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterPublishers(builder);

            RegisterSubscribers(builder);
        }

        private void RegisterPublishers(ContainerBuilder builder)
        {
            builder.RegisterJsonRabbitPublisher<FriendReferralEvent>(
                _connString,
                FriendReferredExchangeName);

            builder.RegisterJsonRabbitPublisher<EmailMessageEvent>(
                _connString,
                NotificationSystemEmailExchangeName);

            builder.RegisterJsonRabbitPublisher<SmsEvent>(
                _connString,
                NotificationSystemSmsExchangeName);

            builder.RegisterJsonRabbitPublisher<PushNotificationEvent>(
                _connString,
                NotificationSystemPushNotificationExchangeName);

            builder.RegisterJsonRabbitPublisher<LeadStateChangedEvent>(
                _connString,
                LeadStateChangedExchangeName);

            builder.RegisterJsonRabbitPublisher<HotelReferralUsedEvent>(
                _connString,
                HotelReferralUsedExchangeName);

            builder.RegisterJsonRabbitPublisher<PropertyLeadApprovedReferralEvent>(
                _connString,
                PropertyLeadApprovedReferralExchangeName);

            builder.RegisterJsonRabbitPublisher<OfferToPurchaseByLeadEvent>(
                _connString,
                OfferToPurchaseByLeadExchangeName);
        }

        private void RegisterSubscribers(ContainerBuilder builder)
        {
            builder.RegisterType<AccountCreatedSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", UserRegisteredExchangeName);

            builder.RegisterType<EmailVerifiedSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", EmailVerifiedExchangeName);

            if (!_isRealEstateFeatureDisabled)
                RegisterSubscribersForRealEstateFeature(builder);
        }

        private void RegisterSubscribersForRealEstateFeature(ContainerBuilder builder)
        {
            //builder.RegisterType<PropertyLeadApprovedSubscriber>()
            //    .As<IStartStop>()
            //    .SingleInstance()
            //    .WithParameter("connectionString", _connString)
            //    .WithParameter("exchangeName", PropertyLeadApprovedExchangeName);

            //builder.RegisterType<OfferToPurchaseByLeadSubscriber>()
            //    .As<IStartStop>()
            //    .SingleInstance()
            //    .WithParameter("connectionString", _connString)
            //    .WithParameter("exchangeName", OfferToPurchasePropertyByLeadExchangeName);
        }
    }
}
