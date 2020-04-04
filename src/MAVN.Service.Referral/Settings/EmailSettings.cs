using JetBrains.Annotations;

namespace MAVN.Service.Referral.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class EmailSettings
    {
        public string EmailTemplateId { set; get; }
        
        public string SubjectTemplateId { set; get; }
    }
}
