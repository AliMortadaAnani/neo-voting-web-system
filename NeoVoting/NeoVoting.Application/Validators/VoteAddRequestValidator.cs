using FluentValidation;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Application.RequestDTOs.Validators
{
    public class VoteAddRequestValidator : AbstractValidator<VoteAddRequest>
    {
        public VoteAddRequestValidator()
        {
            RuleFor(x => x.ElectionId)
                .NotEmpty().WithMessage("An Election ID must be provided to cast a vote.");

            RuleFor(x => x.GovernorateId)
                .Must(id => Enum.IsDefined(typeof(GovernoratesEnum), id))
                .WithMessage("A valid governorate must be selected.");

            RuleFor(x => x.VoterAge)
                .GreaterThanOrEqualTo(18).WithMessage("The voter must be at least 18 years old to vote.");

            RuleFor(x => x.VoterGender)
                .Must(g => g == 'M' || g == 'F').WithMessage("Voter gender must be either 'M' or 'F'.");


        }
    }
}