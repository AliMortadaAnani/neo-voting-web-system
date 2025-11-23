using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs;

namespace GovernmentSystem.API.Application.Validators
{
    public class GetCandidateRequestDTOValidator : AbstractValidator<GetCandidateRequestDTO>
    {
        public GetCandidateRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotNull().NotEmpty();
        }
    }
}