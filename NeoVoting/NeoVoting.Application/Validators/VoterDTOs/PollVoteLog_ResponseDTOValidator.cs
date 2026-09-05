using FluentValidation;
using NeoVoting.Application.ResponseDTOs.VoterDTOs;

namespace NeoVoting.Application.Validators.VoterDTOs
{
    public class PollVoteLog_ResponseDTOValidator : AbstractValidator<PollVoteLog_ResponseDTO>
    {
        public PollVoteLog_ResponseDTOValidator()
        {
            RuleFor(x => x.VoteId).NotNull();
            RuleFor(x => x.TimestampUTC).NotNull();
        }
    }
}