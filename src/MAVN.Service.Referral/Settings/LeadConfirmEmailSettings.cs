using JetBrains.Annotations;

namespace MAVN.Service.Referral.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class LeadConfirmEmailSettings : EmailSettings
    {
        public string ConfirmLinkFormat { set; get; }
    }
}
