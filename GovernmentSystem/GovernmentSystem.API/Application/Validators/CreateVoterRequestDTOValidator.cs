using FluentValidation;
using GovernmentSystem.API.Domain.Enums;
using GovernmentSystem.Application.RequestDTOs;

namespace GovernmentSystem.Application.Validators
{
    public class CreateVoterRequestDTOValidator : AbstractValidator<CreateVoterRequestDTO>
    {
        public CreateVoterRequestDTOValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);

            // 1. Governorate Validation
            RuleFor(x => x.GovernorateId)
                .Must(id => Enum.IsDefined(typeof(GovernorateId), id))
                .WithMessage("A valid governorate must be selected (1-5).");

            // 2. Age Validation (>= 18)
            RuleFor(x => x.DateOfBirth)
                .Must(BeAtLeast18YearsOld)
                .WithMessage("The voter must be at least 18 years old to vote.");

            // 3. Gender Validation
            RuleFor(x => x.Gender)
                .Must(g => g == 'M' || g == 'F')
                .WithMessage("Voter gender must be either 'M' or 'F'.");
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