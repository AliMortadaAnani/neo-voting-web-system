using FluentValidation;
using NeoVoting.Application.AuthDTOs;

namespace NeoVoting.Application.Validators
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
