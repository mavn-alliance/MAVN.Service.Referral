using System;
using System.Threading.Tasks;
using Lykke.Common.Log;
using MAVN.Service.CustomerManagement.Contract.Events;
using MAVN.Service.Referral.Domain.Services;

namespace MAVN.Service.Referral.DomainServices.Subscribers
{
    public class AccountCreatedSubscriber: RabbitSubscriber<CustomerRegistrationEvent>
    {
        private readonly IFriendReferralService _friendReferralService;

        public AccountCreatedSubscriber(
            string connectionString,
            string exchangeName,
            IFriendReferralService friendReferralService,
            ILogFactory logFactory)
            : base(connectionString, exchangeName, logFactory)
        {
            _friendReferralService = friendReferralService;

            GuidsFieldsToValidate.Add(nameof(CustomerRegistrationEvent.CustomerId));
        }

        public override async Task<(bool isSuccessful, string errorMessage)> ProcessMessageAsync(CustomerRegistrationEvent message)
        {
            // ReferralCode being null is a valid case since this event is used not only in referrals
            if (!string.IsNullOrEmpty(message.ReferralCode))
            {
                return await _friendReferralService.ConfirmAsync(
                    message.ReferralCode,
                    Guid.Parse(message.CustomerId));
            }

            return (true, string.Empty);
        }
    }
}
