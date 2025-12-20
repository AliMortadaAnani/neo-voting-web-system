using FluentValidation;

namespace NeoVoting.Application.RequestDTOs.Validators
{
    public class VoteChoiceAddRequestValidator : AbstractValidator<VoteChoiceAddRequest>
    {
        public VoteChoiceAddRequestValidator()
        {
            RuleFor(x => x.VoteId)
                .NotEmpty().WithMessage("A Vote ID must be provided to cast a vote choice.");

            RuleFor(x => x.CandidateProfileId)
                .NotEmpty().WithMessage("A candidate profile must be selected.");
        }
    }
}