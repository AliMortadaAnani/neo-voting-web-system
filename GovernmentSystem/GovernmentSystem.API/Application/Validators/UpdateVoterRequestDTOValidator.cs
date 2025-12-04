using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.Validators
{
    public class UpdateVoterRequestDTOValidator : AbstractValidator<UpdateVoterRequestDTO>
    {
        public UpdateVoterRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotNull().NotEmpty();

            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();

            RuleFor(x => x.GovernorateId)
                .NotNull()
                .Must(id => id.HasValue && Enum.IsDefined(typeof(GovernorateId), id.Value))
                .WithMessage("A valid governorate must be selected.");

            RuleFor(x => x.DateOfBirth)
                .NotNull()
                .Must(d => d.HasValue && BeAtLeast18YearsOld(d.Value))
                .WithMessage("The voter must be at least 18 years old.");

            RuleFor(x => x.Gender)
                .NotNull()
                .Must(g => g.HasValue && (char.ToUpperInvariant(g.Value) == 'M' || char.ToUpperInvariant(g.Value) == 'F'))
                .WithMessage("Gender must be either 'M' or 'F'.");

            RuleFor(x => x.EligibleForElection)
                .NotNull();

            RuleFor(x => x.ValidToken)
                .NotNull();

            RuleFor(x => x.IsRegistered)
                .NotNull();

            RuleFor(x => x.Voted)
                .NotNull();
        }

        private bool BeAtLeast18YearsOld(DateOnly dob)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dob.Year;
            if (dob > today.AddYears(-age)) age--;
            return age >= 18;
        }
    }
}