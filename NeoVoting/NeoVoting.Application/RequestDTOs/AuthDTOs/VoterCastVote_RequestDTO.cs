namespace NeoVoting.Application.RequestDTOs
{
    public class VoterCastVote_RequestDTO
    {
        public List<Guid>? SelectedCandidateProfileIds { get; set; }
        public Guid? NationalId { get; set; }

        public Guid? VotingToken { get; set; }

        public string? Password { get; set; }
    }
}