using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs;

namespace GovernmentSystem.API.Application.Validators
{
    public class GetVoterRequestDTOValidator : AbstractValidator<GetVoterRequestDTO>
    {
        public GetVoterRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotNull().NotEmpty();
        }
    }
}