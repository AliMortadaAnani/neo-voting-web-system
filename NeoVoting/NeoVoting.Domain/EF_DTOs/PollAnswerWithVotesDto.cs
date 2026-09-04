using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.EF_DTOs
{
    public class PollAnswerWithVotesDto
    {
        public PollAnswer pollAnswer { get; set; }

        public int TotalVotes { get; set; }

        public PollAnswerWithVotesDto()
        {
            pollAnswer = null!;
        }
    }
}
