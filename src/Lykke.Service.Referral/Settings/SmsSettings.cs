using JetBrains.Annotations;

namespace Lykke.Service.Referral.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SmsSettings
    {
        public string SmsTemplateId { set; get; }
    }
}
