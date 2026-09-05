using NeoVoting.Domain.Enums;
using System.Text;

namespace NeoVoting.Domain.Entities
{
    public class Election
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public DateTime NominationStartDate { get; private set; }
        public DateTime NominationEndDate { get; private set; }
        public DateTime VotingStartDate { get; private set; }
        public DateTime VotingEndDate { get; private set; }
        public StatusEnum Status { get; private set; }

        public ICollection<CandidateProfile> CandidateProfiles { get; private set; } = new List<CandidateProfile>(); // 1 election can have many candidate profiles

        public ICollection<Vote> Votes { get; private set; } = new List<Vote>(); // 1 election can have many votes

        public ICollection<EventParticipation> EventParticipations { get; private set; } = new List<EventParticipation>();

        private Election()
        { }

        public static Election Create(string name, DateTime nominationStartDate, DateTime nominationEndDate, DateTime votingStartDate, DateTime votingEndDate)
        {
            // --- Centralized Validation Logic ---
            Validate(name);

            var election = new Election
            {
                Name = name,
                NominationStartDate = nominationStartDate,
                NominationEndDate = nominationEndDate,
                VotingStartDate = votingStartDate,
                VotingEndDate = votingEndDate,
                Status = StatusEnum.Upcoming
            };

            return election;
        }

        public void StartVotingPhase()
        {
            if (Status != StatusEnum.Upcoming)
            {
                throw new InvalidOperationException("Cannot start voting unless the election is in the 'Upcoming' state.");
            }
            Status = StatusEnum.Voting;
        }

        public void EndVotingPhase()
        {
            if (Status != StatusEnum.Voting)
            {
                throw new InvalidOperationException("Cannot end voting phase unless the election is in the 'Voting' state.");
            }
            Status = StatusEnum.Completed;
        }

        private static void Validate(
            string name
            )
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.AppendLine("Election name is required.");
            }

            if (errors.Length > 0)
            {
                throw new ArgumentException(errors.ToString());
            }
        }
    }
}