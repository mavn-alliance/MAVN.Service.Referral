using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.Referral.Client.Models.Requests;

namespace MAVN.Service.Referral.Models.Validation
{
    [UsedImplicitly]
    public class ReferralLeadConfirmRequestValidator : AbstractValidator<ReferralLeadConfirmRequest>
    {
        public ReferralLeadConfirmRequestValidator()
        {
            RuleFor(x => x.ConfirmationToken)
                .NotNull()
                .NotEmpty()
                .WithMessage("Confirmation token id is required.");
        }
    }
}
