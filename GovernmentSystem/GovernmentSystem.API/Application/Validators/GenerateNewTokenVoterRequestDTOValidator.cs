using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs;

namespace GovernmentSystem.API.Application.Validators
{
    public class GenerateNewTokenVoterRequestDTOValidator : AbstractValidator<GenerateNewTokenVoterRequestDTO>
    {
        public GenerateNewTokenVoterRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotNull().NotEmpty();
        }
    }
}