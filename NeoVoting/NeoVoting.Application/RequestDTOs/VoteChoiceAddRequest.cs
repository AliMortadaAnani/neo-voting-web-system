namespace NeoVoting.Application.RequestDTOs
{
    /// <summary>
    /// Represents the data required to add a candidate choice to an existing vote.
    /// </summary>
    public class VoteChoiceAddRequest
    {
        // The vote this choice belongs to. This would likely come from the URL.
        public Guid VoteId { get; set; }

        public Guid CandidateProfileId { get; set; }
    }
}