using JetBrains.Annotations;

namespace Lykke.Service.Referral.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class LeadConfirmSmsSettings : SmsSettings
    {
        public string ConfirmLinkFormat { set; get; }
    }
}
