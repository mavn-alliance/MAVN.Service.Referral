using JetBrains.Annotations;

namespace Lykke.Service.Referral.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PushNotificationSettings
    {
        public string TemplateId { get; set; }
        
        public string DeepLinkRoute { get; set; }
    }
}