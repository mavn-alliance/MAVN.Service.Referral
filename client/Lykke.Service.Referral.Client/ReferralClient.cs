using Lykke.HttpClientGenerator;

namespace Lykke.Service.Referral.Client
{
    /// <summary>
    /// Referral API aggregating interface.
    /// </summary>
    public class ReferralClient : IReferralClient
    {
        // Note: Add similar ReferralApi properties for each new service controller

        /// <summary>Interface to Referral ReferralApi.</summary>
        public IReferralApi ReferralApi { get; private set; }

        /// <summary>Interface to Referral ReferralLeadApi.</summary>
        public IReferralLeadApi ReferralLeadApi { get; }

        /// <summary>Interface to Referral ReferralHotelsApi.</summary>
        public IReferralHotelsApi ReferralHotelsApi { get; }

        /// <summary>Application ReferralFriendsApi interface</summary>
        public IReferralFriendsApi ReferralFriendsApi { get; }

        /// <summary> Application CommonReferralApi interface</summary>
        public ICommonReferralApi CommonReferralApi { get; }

        /// <summary>C-tor</summary>
        public ReferralClient(IHttpClientGenerator httpClientGenerator)
        {
            ReferralApi = httpClientGenerator.Generate<IReferralApi>();
            ReferralLeadApi = httpClientGenerator.Generate<IReferralLeadApi>();
            ReferralHotelsApi = httpClientGenerator.Generate<IReferralHotelsApi>();
            ReferralFriendsApi = httpClientGenerator.Generate<IReferralFriendsApi>();
            CommonReferralApi = httpClientGenerator.Generate<ICommonReferralApi>();
        }
    }
}
