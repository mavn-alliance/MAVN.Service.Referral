using JetBrains.Annotations;

namespace MAVN.Service.Referral.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class LeadConfirmSmsSettings : SmsSettings
    {
        public string ConfirmLinkFormat { set; get; }
    }
}
