using FluentValidation;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class Authentication_ResponseDTO_Validator : AbstractValidator<Authentication_ResponseDTO>
    {
        public Authentication_ResponseDTO_Validator()
        {
            // Required fields (non-nullable)
            RuleFor(x => x.AccessToken).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
            RuleFor(x => x.AccessTokenExpiration).NotEmpty();
            RuleFor(x => x.RefreshTokenExpiration).NotEmpty();

            // Optional fields (nullable) - no validation rules needed
            // Id, UserName, FirstName, LastName, GovernorateId, DateOfBirth, Gender, Role
        }
    }
}