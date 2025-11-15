namespace NeoVoting.Application.RequestDTOs
{
    /// <summary>
    /// Represents the data required to update the vote count for an existing election winner.
    /// </summary>
    public class ElectionWinnerVoteCountUpdateRequest
    {
        // Included for validation against the URL route parameter.
        public int Id { get; set; }

        public int VoteCount { get; set; }


    }



}

