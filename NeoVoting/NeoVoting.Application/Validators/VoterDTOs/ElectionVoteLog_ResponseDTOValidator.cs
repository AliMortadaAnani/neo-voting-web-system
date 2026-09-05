using FluentValidation;
using NeoVoting.Application.ResponseDTOs.VoterDTOs;

namespace NeoVoting.Application.Validators.VoterDTOs
{
    public class ElectionVoteLog_ResponseDTOValidator : AbstractValidator<ElectionVoteLog_ResponseDTO>
    {
        public ElectionVoteLog_ResponseDTOValidator()
        {
            RuleFor(x => x.VoteId).NotNull();
            RuleFor(x => x.TimestampUTC).NotNull();
        }
    }
}