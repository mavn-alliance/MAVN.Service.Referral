using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.Referral.Client.Models.Requests;

namespace Lykke.Service.Referral.Models.Validation
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
