using FluentValidation;
using GovernmentSystem.API.Domain.Shared;
using GovernmentSystem.Application.RequestDTOs;

namespace GovernmentSystem.Application.Validators
{
    public class CreateCandidateRequestDTOValidator : AbstractValidator<CreateCandidateRequestDTO>
    {
        public CreateCandidateRequestDTOValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);

            RuleFor(x => x.GovernorateId)
                .Must(id => Enum.IsDefined(typeof(GovernorateId), id))
                .WithMessage("A valid governorate must be selected.");

            // Candidates might have a higher age requirement (e.g., 21 or 25), 
            // but using 18 as requested for now.
            RuleFor(x => x.DateOfBirth)
                .Must(BeAtLeast18YearsOld)
                .WithMessage("The candidate must be at least 18 years old.");

            RuleFor(x => x.Gender)
                .Must(g => g == 'M' || g == 'F')
                .WithMessage("Gender must be either 'M' or 'F'.");
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