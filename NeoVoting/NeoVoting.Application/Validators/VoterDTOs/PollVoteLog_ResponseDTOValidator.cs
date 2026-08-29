using FluentValidation;
using NeoVoting.Application.ResponseDTOs.VoterDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.VoterDTOs
{
    public class PollVoteLog_ResponseDTOValidator : AbstractValidator<PollVoteLog_ResponseDTO>
    {
        public PollVoteLog_ResponseDTOValidator()
        {
            RuleFor(x => x.VoteId).NotNull();
            RuleFor(x => x.PollId).NotNull();
            RuleFor(x => x.PollName).NotEmpty();
            RuleFor(x => x.TimestampUTC).NotNull();
        }
    }
}
