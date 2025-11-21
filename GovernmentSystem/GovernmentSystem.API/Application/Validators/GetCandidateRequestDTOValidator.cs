using FluentValidation;
using GovernmentSystem.Application.RequestDTOs;

namespace GovernmentSystem.Application.Validators
{
    public class GetCandidateRequestDTOValidator : AbstractValidator<GetCandidateRequestDTO>
    {
        public GetCandidateRequestDTOValidator()
        {
            RuleFor(x => x.NationalId).NotEmpty();
        }
    }
}