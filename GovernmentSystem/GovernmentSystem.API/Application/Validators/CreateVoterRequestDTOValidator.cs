using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs;
using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Application.Validators
{
    public class CreateVoterRequestDTOValidator : AbstractValidator<CreateVoterRequestDTO>
    {
        public CreateVoterRequestDTOValidator()
        {
            // ADD THIS TEMPORARILY
            //Console.WriteLine("--> VALIDATORS LOADED: AddVoterValidator...");

            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);

            // 1. Governorate Validation
            RuleFor(x => x.GovernorateId)
                .NotNull()
                .Must(id => id.HasValue && Enum.IsDefined(typeof(GovernorateId), id.Value))
                .WithMessage("A valid governorate must be selected (1-5).");

            // 2. Age Validation (>= 18)
            RuleFor(x => x.DateOfBirth)
                .NotNull()
                .Must(d => d.HasValue && BeAtLeast18YearsOld(d.Value))
                .WithMessage("The voter must be at least 18 years old to vote.");

            // 3. Gender Validation
            RuleFor(x => x.Gender)
                .NotNull()
                .Must(g => g.HasValue && (char.ToUpperInvariant(g.Value) == 'M' || char.ToUpperInvariant(g.Value) == 'F'))
                .WithMessage("Gender must be either 'M' or 'F'.");

            RuleFor(x => x.EligibleForElection)
                .NotNull();
        }

        // Helper for DateOnly Age Calculation
        private bool BeAtLeast18YearsOld(DateOnly dob)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dob.Year;
            if (dob > today.AddYears(-age)) age--;
            return age >= 18;
        }
    }
}