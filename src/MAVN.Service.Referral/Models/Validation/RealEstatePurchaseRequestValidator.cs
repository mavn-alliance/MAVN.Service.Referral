using System;
using FluentValidation;
using MAVN.Service.Referral.Client.Models.Requests;

namespace MAVN.Service.Referral.Models.Validation
{
    public class RealEstatePurchaseRequestValidator: AbstractValidator<RealEstatePurchaseRequest>
    {
        public RealEstatePurchaseRequestValidator()
        {
            RuleFor(x => x.ReferralId)
                .Must(r => Guid.TryParse(r, out var guidId))
                .WithMessage("The Referral Id should be valid GUID.");

            RuleFor(x => x.Timestamp)
                .Must(r => r > new DateTime())
                .WithMessage("The Timestamp is required and should be valid date.");
        }
    }
}
