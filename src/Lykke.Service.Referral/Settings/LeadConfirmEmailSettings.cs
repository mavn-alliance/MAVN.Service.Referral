using JetBrains.Annotations;

namespace Lykke.Service.Referral.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class LeadConfirmEmailSettings : EmailSettings
    {
        public string ConfirmLinkFormat { set; get; }
    }
}
