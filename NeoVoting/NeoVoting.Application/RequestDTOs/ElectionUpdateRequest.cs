namespace NeoVoting.Application.RequestDTOs
{
    /// <summary>
    /// Represents the data required to update an existing election.
    /// </summary>
    public class ElectionUpdateRequest
    {
        // Included for validation against the URL route parameter.
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public DateTime NominationStartDate { get; set; }
        public DateTime NominationEndDate { get; set; }
        public DateTime VotingStartDate { get; set; }
        public DateTime VotingEndDate { get; set; }
    }
}
