using FluentValidation;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class RefreshToken_RequestDTO_Validator : AbstractValidator<RefreshToken_RequestDTO>
    {
        public RefreshToken_RequestDTO_Validator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");

            RuleFor(x => x.AccessToken)
                .NotEmpty().WithMessage("Access token is required.");
        }
    }
}