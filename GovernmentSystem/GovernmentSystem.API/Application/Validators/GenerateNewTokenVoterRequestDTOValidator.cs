using FluentValidation;
using GovernmentSystem.Application.RequestDTOs;

namespace GovernmentSystem.Application.Validators
{
    public class GenerateNewTokenVoterRequestDTOValidator : AbstractValidator<GenerateNewTokenVoterRequestDTO>
    {
        public GenerateNewTokenVoterRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotEmpty();
        }
    }
}