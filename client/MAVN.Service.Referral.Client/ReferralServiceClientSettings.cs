using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.Referral.Client 
{
    /// <summary>
    /// Referral client settings.
    /// </summary>
    public class ReferralServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
