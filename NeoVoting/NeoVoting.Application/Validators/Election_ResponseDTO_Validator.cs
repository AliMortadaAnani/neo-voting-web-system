using FluentValidation;
using NeoVoting.Application.ResponseDTOs;

namespace NeoVoting.Application.Validators
{
    public class Election_ResponseDTO_Validator : AbstractValidator<Election_ResponseDTO>
    {
        public Election_ResponseDTO_Validator()
        {
            // Required fields (non-nullable)
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.NominationStartDate).NotEmpty();
            RuleFor(x => x.NominationEndDate).NotEmpty();
            RuleFor(x => x.VotingStartDate).NotEmpty();
            RuleFor(x => x.VotingEndDate).NotEmpty();
            RuleFor(x => x.ElectionStatusId).NotEmpty();
            RuleFor(x => x.ElectionStatusName).NotEmpty();
        }
    }
}
