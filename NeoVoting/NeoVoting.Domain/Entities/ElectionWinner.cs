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
        public int? VoteCount { get; private set; } // Nullable to allow for recounts or non-applicable counts.

        // --- Foreign Keys & Navigation Properties ---

        public Guid ElectionId { get; private set; }
        public Election Election { get; private set; }

        public Guid CandidateProfileId { get; private set; }
        public CandidateProfile CandidateProfile { get; private set; }


        // --- Constructor ---

        /// <summary>
        /// A private constructor to force all object creation to go through the
        /// controlled, static factory method. EF Core uses this for materializing.
        /// </summary>
        private ElectionWinner()
        {
            // Initialize non-nullable navigation properties to satisfy the C# compiler.
            // EF Core will populate these from the database.
            Election = null!;
            CandidateProfile = null!;
        }


        // --- ToString() Override ---

        /// <summary>
        /// Provides a clear, single-line string representation of the election winner.
        /// </summary>
        /// <returns>A string summary of the winner record.</returns>
        public override string ToString()
        {
            string voteCountDisplay = VoteCount.HasValue ? VoteCount.Value.ToString() : "N/A";
            return $"ElectionWinner [Id: {Id}, ElectionId: {ElectionId}, CandidateId: {CandidateProfileId}, Votes: {voteCountDisplay}]";
        }


        // --- Factory Method ---

        /// <summary>
        /// Creates a new, valid ElectionWinner instance.
        /// </summary>
        /// <param name="electionId">The ID of the election that was won.</param>
        /// <param name="candidateProfileId">The ID of the winning candidate's profile.</param>
        /// <param name="voteCount">The final vote count for the winner, if available.</param>
        /// <returns>A new, valid ElectionWinner object.</returns>
        /// <exception cref="ArgumentException">Thrown if validation fails.</exception>
        public static ElectionWinner Create(Guid electionId, Guid candidateProfileId, int? voteCount)
        {
            Validate(electionId, candidateProfileId, voteCount);

            var winner = new ElectionWinner
            {
                // The 'Id' is typically database-generated (identity column), so we don't set it here.
                // done : check post incremental ids in db and Ef core configuration if needed. (AdminLog,PublicLog, ElectionWinner)
                ElectionId = electionId,
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


        /// <summary>
        /// Private helper method to contain all validation rules.
        /// </summary>
        private static void Validate(Guid electionId, Guid candidateProfileId, int? voteCount)
        {
            var errors = new StringBuilder();

            if (electionId == Guid.Empty)
            {
                errors.AppendLine("ElectionId is required.");
            }

            if (candidateProfileId == Guid.Empty)
            {
                errors.AppendLine("CandidateProfileId is required.");
            }

            // A vote count, if it exists, cannot be a negative number.
            if (voteCount.HasValue && voteCount < 0)
            {
                errors.AppendLine("Vote count cannot be negative.");
            }

            if (errors.Length > 0)
            {
                throw new ArgumentException(errors.ToString());
            }
        }
    }
}
