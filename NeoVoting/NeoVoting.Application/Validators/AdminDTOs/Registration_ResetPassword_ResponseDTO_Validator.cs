using FluentValidation;

namespace NeoVoting.Application.Validators.AdminDTOs
{
    public class Registration_ResetPassword_ResponseDTO_Validator : AbstractValidator<RegisterVoterOrCandidate_ResponseDTO>
    {
        public Registration_ResetPassword_ResponseDTO_Validator()
        {
            // All fields are nullable/optional in this DTO
            // Id, UserName, FirstName, LastName, GovernorateId, DateOfBirth, Gender, Role
        }
    }
}