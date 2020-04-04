using System.Text.RegularExpressions;
using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.Referral.Client.Models.Requests;

namespace MAVN.Service.Referral.Models.Validation
{
    [UsedImplicitly]
    public class ReferralLeadCreateRequestValidator : AbstractValidator<ReferralLeadCreateRequest>
    {
        private static readonly Regex NameRegex = new Regex(@"^((?![1-9!@#$%^&*()_+{}|:\""?></,;[\]\\=~]).)+$");

        public ReferralLeadCreateRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer id is required.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MaximumLength(100)
                .WithMessage("First name shouldn't be more than 100 characters long.")
                .Must(o => NameRegex.IsMatch(o))
                .WithMessage("First name field can contains only letters, periods, hyphens and single quotes.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MaximumLength(100)
                .WithMessage("First name shouldn't be more than 100 characters long.")
                .Must(o => NameRegex.IsMatch(o))
                .WithMessage("Last name field can contains only letters, periods, hyphens and single quotes.");

            RuleFor(x => x.PhoneCountryCodeId)
                .GreaterThan(0)
                .WithMessage("Phone country code id should be greater than 0.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Length(2, 50)
                .WithMessage("Phone number length should be in between 2 and 50 characters long.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email should be a valid email address.");

            RuleFor(o => o.Note)
                .MaximumLength(2000)
                .WithMessage("Note shouldn't be longer than 2000 characters.");
        }
    }
}
