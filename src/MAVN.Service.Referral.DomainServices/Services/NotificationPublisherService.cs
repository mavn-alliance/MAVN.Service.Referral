using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.NotificationSystem.SubscriberContract;
using Lykke.Service.UrlShortener.Client;
using Lykke.Service.UrlShortener.Client.Models;
using MAVN.Service.Referral.Domain.Services;

namespace MAVN.Service.Referral.DomainServices.Services
{
    public class NotificationPublisherService : INotificationPublisherService
    {
        private readonly IRabbitPublisher<PushNotificationEvent> _pushNotificationPublisher;
        private readonly IRabbitPublisher<EmailMessageEvent> _emailsPublisher;
        private readonly IRabbitPublisher<SmsEvent> _smsPublisher;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IUrlShortenerClient _shortenerClient;

        private readonly string _hotelReferralConfirmEmailLinkFormat;
        private readonly string _hotelReferralConfirmHotelReservationLink;
        private readonly string _hotelReferralConfirmEmailTemplateId;
        private readonly string _hotelReferralConfirmEmailSubjectTemplateId;

        private readonly string _friendReferralConfirmEmailTemplateId;
        private readonly string _friendReferralConfirmEmailSubjectTemplateId;

        private readonly string _leadConfirmSmsLinkFormat;
        private readonly string _leadConfirmSmsTemplateId;
        
        private readonly string _leadAlreadyConfirmedEmailTemplateId;
        private readonly string _leadAlreadyConfirmedEmailSubjectTemplateId;
        
        private readonly string _leadSuccessfullyConfirmedEmailTemplateId;
        private readonly string _leadSuccessfullyConfirmedEmailSubjectTemplateId;
        
        private readonly string _leadSuccessfullyConfirmedPushNotificationTemplateId;

        private readonly string _deepLinkReferralList;
        private readonly string _friendConfirmLinkFormat;
        private readonly bool _useShortenedLeadUrl;

        private const string DeepLinkRouteKey = "route";

        public NotificationPublisherService(
            IRabbitPublisher<EmailMessageEvent> emailsPublisher,
            IRabbitPublisher<SmsEvent> smsPublisher,
            IRabbitPublisher<PushNotificationEvent> pushNotificationPublisher,
            ICustomerProfileClient customerProfileClient,
            IUrlShortenerClient shortenerClient,
            string hotelReferralConfirmEmailLinkFormat,
            string hotelReferralConfirmHotelReservationLink,
            string hotelReferralConfirmEmailTemplateId,
            string hotelReferralConfirmEmailSubjectTemplateId,
            string friendReferralConfirmEmailTemplateId,
            string friendReferralConfirmEmailSubjectTemplateId,
            string leadConfirmSmsLinkFormat,
            string leadConfirmSmsTemplateId,
            string leadAlreadyConfirmedEmailTemplateId,
            string leadAlreadyConfirmedEmailSubjectTemplateId,
            string leadSuccessfullyConfirmedEmailTemplateId,
            string leadSuccessfullyConfirmedEmailSubjectTemplateId,
            string leadSuccessfullyConfirmedPushNotificationTemplateId,
            string deepLinkReferralList,
            string friendConfirmLinkFormat,
            bool useShortenedLeadUrl)
        {
            _pushNotificationPublisher = pushNotificationPublisher;
            _emailsPublisher = emailsPublisher;
            _smsPublisher = smsPublisher;
            _customerProfileClient = customerProfileClient;
            _shortenerClient = shortenerClient;

            _hotelReferralConfirmEmailLinkFormat = hotelReferralConfirmEmailLinkFormat;
            _hotelReferralConfirmHotelReservationLink = hotelReferralConfirmHotelReservationLink;
            _hotelReferralConfirmEmailTemplateId = hotelReferralConfirmEmailTemplateId;
            _hotelReferralConfirmEmailSubjectTemplateId = hotelReferralConfirmEmailSubjectTemplateId;

            _friendReferralConfirmEmailTemplateId = friendReferralConfirmEmailTemplateId;
            _friendReferralConfirmEmailSubjectTemplateId = friendReferralConfirmEmailSubjectTemplateId;

            _leadConfirmSmsLinkFormat = leadConfirmSmsLinkFormat;
            _leadConfirmSmsTemplateId = leadConfirmSmsTemplateId;

            _leadAlreadyConfirmedEmailTemplateId = leadAlreadyConfirmedEmailTemplateId;
            _leadAlreadyConfirmedEmailSubjectTemplateId = leadAlreadyConfirmedEmailSubjectTemplateId;

            _leadSuccessfullyConfirmedEmailTemplateId = leadSuccessfullyConfirmedEmailTemplateId;
            _leadSuccessfullyConfirmedEmailSubjectTemplateId = leadSuccessfullyConfirmedEmailSubjectTemplateId;
            _leadSuccessfullyConfirmedPushNotificationTemplateId = leadSuccessfullyConfirmedPushNotificationTemplateId;
            
            _deepLinkReferralList = deepLinkReferralList;
            _friendConfirmLinkFormat = friendConfirmLinkFormat;
            _useShortenedLeadUrl = useShortenedLeadUrl;
        }
        
        public async Task LeadConfirmRequestAsync(string agentCustomerId, string leadPhone, string confirmToken)
        {
            var link = string.Format(_leadConfirmSmsLinkFormat, confirmToken);
            
            if (_useShortenedLeadUrl)
            {
                var shortLink = await _shortenerClient.UrlShorteningApi.ShortenUrlAsync(new ShortenUrlRequestModel
                {
                    Url = link
                });

                link = shortLink.ShortenedUrl;
            }

            var dict = new Dictionary<string, string>
            {
                {"AcceptLink", link}
            };

            await SendSmsAsync(agentCustomerId, leadPhone, dict, _leadConfirmSmsTemplateId);
        }

        private async Task SendSmsAsync(string customerId, string destination, Dictionary<string, string> values, string leadConfirmSmsTemplateId)
        {
            if (!string.IsNullOrWhiteSpace(destination))
                values["target_phonenumber"] = destination;

            await _smsPublisher.PublishAsync(new SmsEvent
            {
                CustomerId = customerId,
                MessageTemplateId = leadConfirmSmsTemplateId,
                TemplateParameters = values,
                Source = $"{AppEnvironment.Name} - {AppEnvironment.Version}"
            });
        }

        public async Task LeadAlreadyConfirmedAsync(string agentCustomerId, string leadFirstName, string leadLastName, string leadPhone)
        {
            var dict = new Dictionary<string, string>
            {
                {"LeadFirstname", leadFirstName},
                {"LeadLastname", leadLastName},
                {"LeadPhone", leadPhone}
            };
            
            await SendEmailAsync(agentCustomerId, null, dict, _leadAlreadyConfirmedEmailTemplateId, _leadAlreadyConfirmedEmailSubjectTemplateId);
        }

        public async Task LeadSuccessfullyConfirmedAsync(string agentCustomerId, string leadFirstName, string leadLastName, string leadPhone)
        {
            var emailParams = new Dictionary<string, string>
            {
                {"LeadFirstname", leadFirstName},
                {"LeadLastname", leadLastName},
                {"LeadPhone", leadPhone}
            };
            
            await SendEmailAsync(agentCustomerId, null, emailParams, _leadSuccessfullyConfirmedEmailTemplateId, _leadSuccessfullyConfirmedEmailSubjectTemplateId);
            
            var pushParams = new Dictionary<string, string>
            {
                {"LeadFirstname", leadFirstName},
                {"LeadLastname", leadLastName}
            };
            
            var customPayload = new Dictionary<string, string> {[DeepLinkRouteKey] = _deepLinkReferralList};
            
            await SendPushNotificationAsync(agentCustomerId, _leadSuccessfullyConfirmedPushNotificationTemplateId,
                customPayload, pushParams);
        }

        public async Task HotelReferralConfirmRequestAsync(string referrerId, string leadEmail, TimeSpan timeWhileInvitationValid,
            string confirmationToken)
        {
            var dict = new Dictionary<string, string>
            {
                {"HotelReservationLink", _hotelReferralConfirmHotelReservationLink},
                {"TimeWhileInvitationValid", ToReadableString(timeWhileInvitationValid)},
                {"ConfirmationLink", string.Format(_hotelReferralConfirmEmailLinkFormat, confirmationToken)}
            };

            await SendEmailAsync(referrerId, leadEmail, dict, _hotelReferralConfirmEmailTemplateId, _hotelReferralConfirmEmailSubjectTemplateId);
        }

        public async Task FriendReferralConfirmRequestAsync(
            string agentCustomerId, 
            string referralCode, 
            string referrerFirstName, 
            string referrerLastName,
            string leadEmail)
        {
            var dict = new Dictionary<string, string>
            {
                {"ReferrerFirstname", referrerFirstName},
                {"ReferrerLastname", referrerLastName},
                {"ConfirmationLink", string.Format(_friendConfirmLinkFormat, referralCode)}
            };

            await SendEmailAsync(agentCustomerId, leadEmail, dict, _friendReferralConfirmEmailTemplateId, _friendReferralConfirmEmailSubjectTemplateId);
        }

        private async Task SendEmailAsync(string customerId, string destination, Dictionary<string, string> values, string emailTemplateId, string subjectTemplateId)
        {
            if(!string.IsNullOrWhiteSpace(destination))
                values["target_email"] = destination;
            
            await _emailsPublisher.PublishAsync(new EmailMessageEvent
            {
                CustomerId = customerId,
                MessageTemplateId = emailTemplateId,
                SubjectTemplateId = subjectTemplateId,
                TemplateParameters = values,
                Source = $"{AppEnvironment.Name} - {AppEnvironment.Version}"
            });
        }

        private async Task SendPushNotificationAsync(string customerId, string messageTemplateId, Dictionary<string, string> customPayload, Dictionary<string, string> templateParams)
        {
            await _pushNotificationPublisher.PublishAsync(new PushNotificationEvent
            {
                CustomerId = customerId,
                CustomPayload = customPayload,
                MessageTemplateId = messageTemplateId,
                TemplateParameters = templateParams,
                Source = $"{AppEnvironment.Name} - {AppEnvironment.Version}"
            });
        }
        
        private static string ToReadableString(TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? String.Empty : "s") : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

            return formatted;
        }
    }
}
