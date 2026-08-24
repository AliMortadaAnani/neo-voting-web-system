using FluentValidation;
using GovernmentSystem.API.Application.RequestDTOs.CandidateDTOs;

namespace GovernmentSystem.API.Application.Validators.CandidateDTOs
{
    public class GetCandidateRequestDTOValidator : AbstractValidator<GetCandidateRequestDTO>
    {
        public GetCandidateRequestDTOValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}