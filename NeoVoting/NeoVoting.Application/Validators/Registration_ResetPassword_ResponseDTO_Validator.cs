using FluentValidation;
using NeoVoting.Application.AuthDTOs;

namespace NeoVoting.Application.Validators
{
    public class Registration_ResetPassword_ResponseDTO_Validator : AbstractValidator<Registration_ResetPassword_ResponseDTO>
    {
        public Registration_ResetPassword_ResponseDTO_Validator()
        {
            // All fields are nullable/optional in this DTO
            // Id, UserName, FirstName, LastName, GovernorateId, DateOfBirth, Gender, Role
        }
    }
}
