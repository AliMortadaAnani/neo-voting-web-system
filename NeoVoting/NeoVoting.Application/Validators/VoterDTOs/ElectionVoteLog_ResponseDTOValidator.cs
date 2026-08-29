using FluentValidation;
using NeoVoting.Application.ResponseDTOs.VoterDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.VoterDTOs
{
    public class ElectionVoteLog_ResponseDTOValidator : AbstractValidator<ElectionVoteLog_ResponseDTO>
    {
        public ElectionVoteLog_ResponseDTOValidator()
        {
            RuleFor(x => x.VoteId).NotNull();
            RuleFor(x => x.ElectionId).NotNull();
            RuleFor(x => x.ElectionName).NotEmpty();
            RuleFor(x => x.TimestampUTC).NotNull();
            RuleFor(x => x.GovernorateId).NotNull();
        }
    }
}
