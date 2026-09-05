using NeoVoting.Domain.Entities;

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