using JetBrains.Annotations;

namespace MAVN.Service.Referral.Client
{
    /// <summary>
    /// Referral client interface.
    /// </summary>
    [PublicAPI]
    public interface IReferralClient
    {
        /// <summary>Application ReferralApi interface</summary>
        IReferralApi ReferralApi { get; }

        /// <summary>Application ReferralLeadApi interface</summary>
        IReferralLeadApi ReferralLeadApi { get; }

        /// <summary>Application ReferralHotelsApi interface</summary>
        IReferralHotelsApi ReferralHotelsApi { get; }

        /// <summary>Application ReferralFriendsApi interface</summary>
        IReferralFriendsApi ReferralFriendsApi { get; }

        /// <summary> Application CommonReferralApi interface</summary>
        ICommonReferralApi CommonReferralApi { get; }
    }
}
