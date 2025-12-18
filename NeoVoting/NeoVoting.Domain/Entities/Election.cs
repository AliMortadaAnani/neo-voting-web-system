using NeoVoting.Domain.Enums;
using System.Text;


namespace NeoVoting.Domain.Entities
{
    public class Election
    {
        
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public DateTime NominationStartDate { get; private set; }
        public DateTime NominationEndDate { get; private set; }
        public DateTime VotingStartDate { get; private set; }
        public DateTime VotingEndDate { get; private set; }

        public int? FinalNumberOfRegisteredVoters { get; private set; }
        // number of registered voters in the system just after this election is completed

        // --- Foreign Key & Navigation Property ---

        public int ElectionStatusId { get; private set; } 
        public ElectionStatus ElectionStatus { get; private set; }

        
        private Election()
        {
            ElectionStatus = null!;
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
            Validate(name, nominationStartDate, nominationEndDate, votingStartDate, votingEndDate,isCreating:true);

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
        /// Moves the election to the Nomination phase.
        /// Throws an exception if the election is not in the 'Upcoming' state.
        /// </summary>
        public void StartNomination()
        {
            if (ElectionStatusId != (int)ElectionStatusEnum.Upcoming)
            {
                throw new InvalidOperationException("Cannot start nomination unless the election is in the 'Upcoming' state.");
            }
            ElectionStatusId = (int)ElectionStatusEnum.Nomination;
        }

        /// <summary>
        /// Moves the election to the pre-Voting phase.
        /// Throws an exception if the election is not in the 'Nomination' state.
        /// </summary>
        public void StartPreVotingPhase()
        {
            if (ElectionStatusId != (int)ElectionStatusEnum.Nomination)
            {
                throw new InvalidOperationException("Cannot start pre-voting phase unless the election is in the 'Nomination' state.");
            }
            ElectionStatusId = (int)ElectionStatusEnum.PreVotingPhase;
        }

        /// <summary>
        /// Moves the election to the Voting phase.
        /// Throws an exception if the election is not in the 'pre-Voting' state.
        /// </summary>
        public void StartVoting()
        {
            if (ElectionStatusId != (int)ElectionStatusEnum.PreVotingPhase)
            {
                throw new InvalidOperationException("Cannot start voting unless the election is in the 'pre-Voting' state.");
            }
            ElectionStatusId = (int)ElectionStatusEnum.Voting;
        }

        /// <summary>
        /// Moves the election to the Completed phase.
        /// Throws an exception if the election is not in the 'Voting' state.
        /// </summary>
        public void CompleteElection(int count)
        {
            if (ElectionStatusId != (int)ElectionStatusEnum.Voting)
            {
                throw new InvalidOperationException("Cannot complete the election unless it is in the 'Voting' state.");
            }
            ElectionStatusId = (int)ElectionStatusEnum.Completed;
            SetNumberOfRegisteredVoters(count);
        }

        //Calculated just after completing the election
        //final calculated number of registered voters
        private void SetNumberOfRegisteredVoters(int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("Number of registered voters cannot be negative.");
            }

            if (ElectionStatusId != (int)ElectionStatusEnum.Completed)
            {
                throw new InvalidOperationException("Cannot update registered voter count before election is completed.");
            }


            FinalNumberOfRegisteredVoters = count;
        }

        /// <summary>
        /// Private helper method to contain all validation rules for creating an election.
        /// </summary>
        private static void Validate(
            string name,
            DateTime nominationStartDate,
            DateTime nominationEndDate,
            DateTime votingStartDate,
            DateTime votingEndDate,
            bool isCreating = false // default false for update
            )
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.AppendLine("Election name is required.");
            }
            //when creating, nomination start date must be in the future
            if (isCreating && nominationStartDate <= DateTime.UtcNow)
            {
                errors.AppendLine("Nomination start date must be in the future.");
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