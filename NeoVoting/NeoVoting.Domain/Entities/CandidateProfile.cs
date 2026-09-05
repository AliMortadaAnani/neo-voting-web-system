using System.Text;

namespace NeoVoting.Domain.Entities
{
    public class CandidateProfile
    {
        public int Id { get; private set; }
        public string NominationReasons { get; private set; } = string.Empty;

        // --- Foreign Keys & Navigation Properties ---

        public int CandidateId { get; private set; } // The user who is the candidate
        public Candidate Candidate { get; private set; }

        public int ElectionId { get; private set; } // The election they are running in

        //same user as candidate can have multiple profiles in different elections (one profile per election to be considered nominated for that election)
        public Election Election { get; private set; }

        // the votes that this candidate has received in the election they are running in
        public ICollection<Vote> Votes { get; private set; } = new List<Vote>();

        private CandidateProfile()
        {
            Candidate = null!;
            Election = null!;
        }

        public static CandidateProfile Create(int candidateId, int electionId, string nominationReasons)
        {
            // --- Centralized Validation Logic ---
            ValidateFields(nominationReasons);

            var profile = new CandidateProfile
            {
                CandidateId = candidateId,
                ElectionId = electionId,
                NominationReasons = nominationReasons
            };

            return profile;
        }

        private static void ValidateFields(string nominationReasons)
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(nominationReasons))
            {
                errors.AppendLine("Nomination reasons are required.");
            }

            if (errors.Length > 0)
            {
                throw new ArgumentException(errors.ToString());
            }
        }
    }
}