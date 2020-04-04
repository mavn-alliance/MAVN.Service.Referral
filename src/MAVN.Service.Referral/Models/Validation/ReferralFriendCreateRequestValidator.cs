using System;
using System.Text.RegularExpressions;
using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.Referral.Client.Models.Requests;

namespace MAVN.Service.Referral.Models.Validation
{
    [UsedImplicitly]
    public class ReferralFriendCreateRequestValidator : AbstractValidator<ReferralFriendCreateRequest>
    {
        private static readonly Regex NameRegex = new Regex(@"^((?![1-9!@#$%^&*()_+{}|:\""?></,;[\]\\=~]).)+$");

        public ReferralFriendCreateRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer id is required.");

            RuleFor(x => x.CampaignId)
                .Must(o => o != Guid.Empty)
                .WithMessage("Customer id is required.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Full name is required.")
                .Length(3, 200)
                .WithMessage("Full name length should be in between 3 and 200 characters long.")
                .Must(o => NameRegex.IsMatch(o))
                .WithMessage("Full name field can contains only letters, periods, hyphens and single quotes.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email should be a valid email address.");
        }
    }
}
