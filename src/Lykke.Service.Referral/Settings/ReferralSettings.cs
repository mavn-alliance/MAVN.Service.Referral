using System;
using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.Referral.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ReferralSettings
    {
        public DbSettings Db { get; set; }

        public RabbitMqSettings RabbitMq { get; set; }

        public string BaseCurrencyCode { get; set; }

        public int ReferralCodeLength { get; set; }

        public int LeadConfirmationTokenLength { get; set; }

        public TimeSpan ReferralHotelLimitPeriod { get; set; }

        public int ReferralHotelLimit { get; set; }

        public TimeSpan ReferralHotelExpirationPeriod { set; get; }

        public HotelReferralConfirmEmailSettings HotelReferralConfirmEmailSettings { get; set; }

        public FriendReferralConfirmEmailSettings FriendReferralConfirmEmailSettings { get; set; }

        public LeadConfirmSmsSettings LeadConfirmSmsSettings { get; set; }

        public EmailSettings LeadAlreadyConfirmedEmailSettings { get; set; }

        public EmailSettings LeadSuccessfullyConfirmedEmailSettings { get; set; }

        public PushNotificationSettings LeadSuccessfullyConfirmedPushNotificationSettings { get; set; }

        public int DemoOperationDelay { get; set; }

        public string EmailDemoIdentifier { get; set; }

        public bool UseShortenedLeadUrl { get; set; }

        [Optional]
        public bool? IsRealEstateFeatureDisabled { get; set; }
    }
}
