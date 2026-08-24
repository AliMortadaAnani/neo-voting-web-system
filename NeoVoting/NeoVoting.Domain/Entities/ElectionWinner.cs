using System.Text;

namespace NeoVoting.Domain.Entities
{
    /// <summary>
    /// Represents the winner of an election. This is a record created after an
    /// election is completed to formally identify the winning candidate.
    /// </summary>
    public class ElectionWinner
    {
        // --- Properties ---

        public int Id { get; private set; }
        public int? VoteCount { get; private set; } 

        // --- Foreign Keys & Navigation Properties ---

       

        public int CandidateProfileId { get; private set; }
        public CandidateProfile CandidateProfile { get; private set; }

        private ElectionWinner()
        {
            // Initialize non-nullable navigation properties to satisfy the C# compiler.
            // EF Core will populate these from the database.
           
            CandidateProfile = null!;
        }


        public static ElectionWinner Create(int candidateProfileId, int? voteCount)
        {
            if(voteCount.HasValue && voteCount < 0)
            {
                throw new ArgumentException("Vote count cannot be negative.", nameof(voteCount));
            }

            var winner = new ElectionWinner
            {
                // The 'Id' is typically database-generated (identity column), so we don't set it here.
                CandidateProfileId = candidateProfileId,
                VoteCount = voteCount
            };

            return winner;
        }

        // --- Public Behavior Methods ---

        /// <summary>
        /// Updates the vote count for this winner. This is useful for scenarios
        /// like entering a tally after the fact or correcting a count after a recount.
        /// </summary>
        /// <param name="newVoteCount">The new, corrected vote count.</param>
        public void UpdateVoteCount(int newVoteCount)
        {
            if (newVoteCount < 0)
            {
                throw new ArgumentException("Vote count cannot be negative.", nameof(newVoteCount));
            }
            this.VoteCount = newVoteCount;
        }

        
    }
}