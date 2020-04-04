using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.Referral.Client.Models.Requests;

namespace MAVN.Service.Referral.Models.Validation
{
    [UsedImplicitly]
    public class ReferralCreateRequestValidator : AbstractValidator<ReferralCreateRequest>
    {
        public ReferralCreateRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Customer id is required");
        }
    }
}
