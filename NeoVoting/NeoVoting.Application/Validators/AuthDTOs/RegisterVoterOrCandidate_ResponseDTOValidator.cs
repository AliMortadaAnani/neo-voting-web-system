using FluentValidation;
using NeoVoting.Application.ResponseDTOs.AuthDTOs;

namespace NeoVoting.Application.Validators.AuthDTOs
{
    public class RegisterVoterOrCandidate_ResponseDTOValidator : AbstractValidator<RegisterVoterOrCandidate_ResponseDTO>
    {
        public RegisterVoterOrCandidate_ResponseDTOValidator()
        {
            RuleFor(x => x.ApplicationUserId).NotNull();
            RuleFor(x => x.AccountId).NotNull();
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Governorate).NotNull();
            RuleFor(x => x.DateOfBirth).NotNull();
            RuleFor(x => x.Gender).NotNull();
            RuleFor(x => x.Role).NotNull();
            RuleFor(x => x.Message).NotEmpty();
        }
    }
}