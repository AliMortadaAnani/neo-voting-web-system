namespace GovernmentSystem.API.Application.Validators
{
    using FluentValidation;
    using GovernmentSystem.API.Application.AdminDTOs;

    // we are validating response DTOs to ensure that documentation tool like Swagger shows correct required fields in the schema
    // Although no actual validation will be performed at runtime for response DTOs
    public class AuthResponseValidator : AbstractValidator<AuthResponse>
    {
        public AuthResponseValidator()
        {
            RuleFor(x => x.IsSuccess)
                .NotNull(); // bool is non-nullable, this mainly affects schema docs

            RuleFor(x => x.Message)
                .NotEmpty();

            RuleFor(x => x.Username)
                .NotEmpty();

            RuleFor(x => x.Role)
                .NotEmpty();
        }
    }
}