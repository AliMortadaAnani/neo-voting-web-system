using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.CitizenDTOs;
using GovernmentSystem.API.Domain.Enums;

namespace GovernmentSystem.API.Application.Validators.CitizenDTOs
{
    public class UpdateCitizenRequestDTOValidator : AbstractValidator<UpdateCitizenRequestDTO>
    {
        public UpdateCitizenRequestDTOValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.DateOfBirth)
                .NotNull()
                 .Must(d => d.HasValue && BeAtLeast18YearsOld(d.Value))
                .WithMessage("The candidate must be at least 18 years old.");

            RuleFor(x => x.Governorate)
                .NotNull()
                .Must(id => id.HasValue && Enum.IsDefined(typeof(GovernorateIdEnum), id.Value));

            RuleFor(x => x.Gender)
                .NotNull()
                .Must(g => g.HasValue && (char.ToUpperInvariant(g.Value) == 'M' || char.ToUpperInvariant(g.Value) == 'F'));
        }

        private bool BeAtLeast18YearsOld(DateOnly dob)
        {
            // Use UtcNow to avoid Server Timezone issues
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var age = today.Year - dob.Year;

            // Check if birthday has passed this year
            if (dob > today.AddYears(-age)) age--;

            return age >= 18;
        }
    }
}