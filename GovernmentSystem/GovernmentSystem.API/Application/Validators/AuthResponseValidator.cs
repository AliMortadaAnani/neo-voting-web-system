namespace GovernmentSystem.API.Application.Validators
{
    using FluentValidation;
    using GovernmentSystem.API.Application.AdminDTOs;

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