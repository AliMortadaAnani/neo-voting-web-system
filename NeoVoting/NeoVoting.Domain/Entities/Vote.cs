using NeoVoting.Domain.Enums; // Assuming enums for Governorates are here
using System.Text;

namespace NeoVoting.Domain.Entities
{
    /// <summary>
    /// Represents a single, immutable vote cast in an election.

    /// </summary>
    public class Vote
    {
        // --- Properties ---

        public Guid Id { get; private set; }
        public int VoterAge { get; private set; }
        public char VoterGender { get; private set; }
        public DateTime TimestampUTC { get; private set; }

        // --- Foreign Keys & Navigation Properties ---

        public Guid ElectionId { get; private set; }
        public Election Election { get; private set; }

        public int GovernorateId { get; private set; }
        public Governorate Governorate { get; private set; }


        // --- Constructor ---

        /// <summary>
        /// A private constructor to force all object creation to go through the
        /// controlled, static factory method. EF Core uses this for materializing.
        /// </summary>
        private Vote()
        {
            Election = null!;
            Governorate = null!;
        }


        // --- ToString() Override ---

        /// <summary>
        /// Provides a detailed, single-line string representation of the vote,
        /// which is extremely useful for debugging and logging.
        /// </summary>
        /// <returns>A comprehensive string summary of the vote.</returns>
        public override string ToString()
        {
            // This format includes all key fields in a structured key-value style for clarity in logs.
            // The 'o' format for the timestamp is the ISO 8601 round-trip format, which is unambiguous.
            return $"Vote [Id: {Id}, ElectionId: {ElectionId}, Voter: {VoterAge}-{VoterGender}, GovId: {GovernorateId}, Timestamp: {TimestampUTC:o}]";
        }


        // --- Factory Method ---

        /// <summary>
        /// Creates a new, valid Vote instance.

        /// </summary>
        /// <param name="electionId">The ID of the election this vote is for.</param>
        /// <param name="governorateId">The ID of the governorate where the vote was cast.</param>
        /// <param name="voterAge">The age of the voter at the time of voting.</param>
        /// <param name="voterGender">The gender of the voter ('M' or 'F').</param>
        /// <returns>A new, valid Vote object.</returns>
        /// <exception cref="ArgumentException">Thrown if validation fails.</exception>
        public static Vote Create(Guid electionId, int governorateId, int voterAge, char voterGender)
        {
            // --- Centralized Validation Logic ---
            Validate(electionId, governorateId, voterAge, voterGender);

            var vote = new Vote
            {
                Id = Guid.NewGuid(),
                ElectionId = electionId,
                GovernorateId = governorateId,
                VoterAge = voterAge,
                VoterGender = char.ToUpper(voterGender),
                TimestampUTC = DateTime.UtcNow // The timestamp is always set at the moment of creation.
            };

            return vote;
        }

        // A Vote, once created, is immutable. There are no "Update" methods.

        /// <summary>
        /// Private helper method to contain all validation rules for creating a vote.
        /// This validates the parameters themselves, not the business context (e.g., if the election is active).
        /// </summary>
        private static void Validate(Guid electionId, int governorateId, int voterAge, char voterGender)
        {
            var errors = new StringBuilder();

            if (electionId == Guid.Empty)
            {
                errors.AppendLine("ElectionId is required.");
            }

            // Assuming 18 is the legal voting age.
            if (voterAge < 18)
            {
                errors.AppendLine("Voter must be at least 18 years old.");
            }

            if (char.ToUpper(voterGender) != 'M' && char.ToUpper(voterGender) != 'F')
            {
                errors.AppendLine("Voter gender must be 'M' or 'F'.");
            }

            if (!Enum.IsDefined(typeof(GovernoratesEnum), governorateId))
            {
                errors.AppendLine("A valid governorate is required.");
            }

            if (errors.Length > 0)
            {
                throw new ArgumentException(errors.ToString());
            }
        }
    }
}