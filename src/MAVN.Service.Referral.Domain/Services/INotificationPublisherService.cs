using System;
using System.Threading.Tasks;

namespace MAVN.Service.Referral.Domain.Services
{
    public interface INotificationPublisherService
    {
        Task LeadConfirmRequestAsync(string agentCustomerId, string leadPhone, string confirmToken);
        Task LeadAlreadyConfirmedAsync(string agentCustomerId, string leadFirstName, string leadLastName, string leadPhone);
        Task LeadSuccessfullyConfirmedAsync(string agentCustomerId, string leadFirstName, string leadLastName, string leadPhone);
        Task HotelReferralConfirmRequestAsync(string referrerId, string leadEmail, TimeSpan timeWhileInvitationValid, string confirmationToken);
        Task FriendReferralConfirmRequestAsync(string agentCustomerId, string referralCode, string referrerFirstName, string referrerLastName, string leadEmail);
    }
}
