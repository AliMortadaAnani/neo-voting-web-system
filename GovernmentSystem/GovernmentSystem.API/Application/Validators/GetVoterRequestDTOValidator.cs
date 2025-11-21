using FluentValidation;
using GovernmentSystem.Application.RequestDTOs;

namespace GovernmentSystem.Application.Validators
{
    public class GetVoterRequestDTOValidator : AbstractValidator<GetVoterRequestDTO>
    {
        public GetVoterRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotEmpty();
        }
    }
}