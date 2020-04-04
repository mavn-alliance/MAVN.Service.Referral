using MAVN.Service.Referral.Domain.Services;

namespace MAVN.Service.Referral.DomainServices.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly int _leadConfirmationTokenLength;
        private readonly string _demoEmailIdentifier;

        public SettingsService(
            int leadConfirmationTokenLength,
            string demoEmailIdentifier)
        {
            _leadConfirmationTokenLength = leadConfirmationTokenLength;
            _demoEmailIdentifier = demoEmailIdentifier;
        }

        public int GetLeadConfirmationTokenLength()
        {
            return _leadConfirmationTokenLength;
        }

        public string GetDemoEmailIdentifier()
        {
            return _demoEmailIdentifier;
        }
    }
}
