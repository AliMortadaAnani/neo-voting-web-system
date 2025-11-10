using NeoVoting.Domain.Enums;
using System.Text;


namespace NeoVoting.Domain.Entities
{
    public class Election
    {
        // --- Properties ---

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime NominationStartDate { get; set; }
        public DateTime NominationEndDate { get; set; }
        public DateTime VotingStartDate { get; set; }
        public DateTime VotingEndDate { get; set; }

        // --- Foreign Key & Navigation Property ---
        public int ElectionStatusId { get; set; }
        public ElectionStatus? ElectionStatus { get; set; }


        // --- Constructor ---

        /// <summary>
        /// A private constructor to force all object creation to go through the
        /// controlled, static factory method. EF Core uses this for materializing.
        /// </summary>
        private Election() { }


        // --- ToString() Override ---

        /// <summary>
        /// Provides a detailed, multi-line string representation of the election's state,
        /// which is extremely useful for debugging and logging.
        /// </summary>
        /// <returns>A comprehensive string summary of the election.</returns>
        public override string ToString()
        {
            
           
            var sb = new StringBuilder();
            sb.AppendLine($"Election: '{Name}' (Id: {Id})");
            sb.AppendLine($"  Status: ({ElectionStatusId}) " +
                $"{((ElectionStatusEnum)ElectionStatusId).GetDescription()}");
            sb.AppendLine($"  Nomination: {NominationStartDate:yyyy-MM-dd HH:mm} to {NominationEndDate:yyyy-MM-dd HH:mm} UTC");
            sb.AppendLine($"  Voting:     {VotingStartDate:yyyy-MM-dd HH:mm} to {VotingEndDate:yyyy-MM-dd HH:mm} UTC");
            return sb.ToString();
        }


        // --- Factory Method ---

        /// <summary>
        /// Creates a new, valid Election instance.
        /// Enforces business rules, such as date ordering, before creation.
        /// </summary>
        /// <param name="name">The official name of the election.</param>
        /// <param name="nominationStartDate">The UTC date when nominations open.</param>
        /// <param name="nominationEndDate">The UTC date when nominations close.</param>
        /// <param name="votingStartDate">The UTC date when voting opens.</param>
        /// <param name="votingEndDate">The UTC date when voting closes.</param>
        /// <returns>A new, valid Election object.</returns>
        /// <exception cref="ArgumentException">Thrown if validation fails.</exception>
        public static Election Create(string name, DateTime nominationStartDate, DateTime nominationEndDate, DateTime votingStartDate, DateTime votingEndDate)
        {
            // --- Centralized Validation Logic ---
            Validate(name, nominationStartDate, nominationEndDate, votingStartDate, votingEndDate);

            var election = new Election
            {
                Id = Guid.NewGuid(),
                Name = name,
                NominationStartDate = nominationStartDate,
                NominationEndDate = nominationEndDate,
                VotingStartDate = votingStartDate,
                VotingEndDate = votingEndDate,
                ElectionStatusId = (int)ElectionStatusEnum.Upcoming // New elections always start as 'Upcoming'.
            };

            return election;
        }

        /// <summary>
        /// Private helper method to contain all validation rules for creating an election.
        /// </summary>
        private static void Validate(string name, DateTime nominationStartDate, DateTime nominationEndDate, DateTime votingStartDate, DateTime votingEndDate)
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.AppendLine("Election name is required.");
            }

            // Rule 1: Nomination end date must be after start date.
            if (nominationEndDate <= nominationStartDate)
            {
                errors.AppendLine("Nomination end date must be after the start date.");
            }

            // Rule 2: Voting start date must be after nomination has ended.
            if (votingStartDate <= nominationEndDate)
            {
                errors.AppendLine("Voting start date must be after the nomination period has ended.");
            }

            // Rule 3: Voting end date must be after voting has started.
            if (votingEndDate <= votingStartDate)
            {
                errors.AppendLine("Voting end date must be after the voting start date.");
            }

            if (errors.Length > 0)
            {
                throw new ArgumentException(errors.ToString());
            }
        }
    }
}