using FluentValidation;
using NeoVoting.Application.RequestDTOs.VoterDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.Validators.VoterDTOs
{
    public class Voter_TrackVote_RequestDTOValidator : AbstractValidator<Voter_TrackVote_RequestDTO>
    {
        public Voter_TrackVote_RequestDTOValidator()
        {
            RuleFor(x => x.VoteId).NotNull();
        }
    }
}
