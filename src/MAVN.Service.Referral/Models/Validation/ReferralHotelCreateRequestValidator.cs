using System.Text.RegularExpressions;
using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.Referral.Client.Models.Requests;

namespace MAVN.Service.Referral.Models.Validation
{
    [UsedImplicitly]
    public class ReferralHotelCreateRequestValidator : AbstractValidator<ReferralHotelCreateRequest>
    {
        public ReferralHotelCreateRequestValidator()
        {
            RuleFor(x => x.ReferrerId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Referrer id is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email should be a valid email address.");
            
            RuleFor(x => x.FullName)
                .NotEmpty()
                .NotNull()
                .WithMessage("FullName id is required.");
            
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .NotNull()
                .WithMessage("PhoneNumber id is required.");

            RuleFor(x => x.PhoneCountryCodeId)
                .GreaterThan(0)
                .WithMessage("Phone country code id should be greater than 0.");
        }
    }
}
