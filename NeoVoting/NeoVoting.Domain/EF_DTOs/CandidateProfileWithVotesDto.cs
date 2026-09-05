using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.EF_DTOs
{
    public class CandidateProfileWithVotesDto
    {
        public CandidateProfile CandidateProfile { get; set; }
        public int TotalVotes { get; set; }

        public CandidateProfileWithVotesDto()
        {
            CandidateProfile = null!;
        }
    }
}