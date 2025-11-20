using GovernmentSystem.API.Domain.Enums;
using System.Text;

namespace GovernmentSystem.API.Domain.Entities
{
    public class Voter
    {
        // 1. Properties
        public Guid Id { get; private set; }
        public Guid NationalId { get; private set; }
        public Guid VotingToken { get; private set; }
        public GovernorateId GovernorateId { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public DateOnly DateOfBirth { get; private set; }
        public char Gender { get; private set; }
        public bool EligibleForElection { get; private set; }
        public bool ValidToken { get; private set; }
        public bool IsRegistered { get; private set; }
        public bool Voted { get; private set; }

        // 2. Private Constructor (EF Core requires this or a binding constructor)
        private Voter() { }

        // 3. Static Create Method
        public static Voter Create(
            GovernorateId governorateId,
            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            char gender,
            bool eligibleForElection)
        {
            ValidateGender(gender);
            ValidateGovernorate(governorateId);

            return new Voter
            {
                Id = Guid.NewGuid(),
                NationalId = Guid.NewGuid(),   // Generated automatically
                VotingToken = Guid.NewGuid(),  // Generated automatically
                GovernorateId = governorateId,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Gender = char.ToUpper(gender),
                EligibleForElection = eligibleForElection,
                ValidToken = true,             // Default to true on creation
                IsRegistered = false,
                Voted = false
            };
        }

        // 4. Update Method (All fields except Id and VotingToken)
        public void UpdateDetails(
            
            GovernorateId governorateId,
            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            char gender,
            bool eligibleForElection,
            bool validToken,
            bool isRegistered,
            bool voted)
        {
            ValidateGender(gender);
            ValidateGovernorate(governorateId);

            
            GovernorateId = governorateId;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = char.ToUpper(gender);
            EligibleForElection = eligibleForElection;
            ValidToken = validToken;
            IsRegistered = isRegistered;
            Voted = voted;
        }

        // 5. Set New Token Method
        public void GenerateNewVotingToken()
        {
            VotingToken = Guid.NewGuid();
            ValidToken = true; // Re-enable token if it was invalid
        }

        // 6. ToString Method
        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine($"[Voter Record]")
                .AppendLine($"Id: {Id}")
                .AppendLine($"NationalID: {NationalId}")
                .AppendLine($"VotingToken: {VotingToken}")
                .AppendLine($"Governorate: {GovernorateId}")
                .AppendLine($"Name: {FirstName} {LastName}")
                .AppendLine($"DOB: {DateOfBirth}")
                .AppendLine($"Gender: {Gender}")
                .AppendLine($"Eligible: {EligibleForElection}")
                .AppendLine($"ValidToken: {ValidToken}")
                .AppendLine($"Registered: {IsRegistered}")
                .AppendLine($"Voted: {Voted}")
                .ToString();
        }

        // Helpers
        private static void ValidateGender(char gender)
        {
            char g = char.ToUpper(gender);
            if (g != 'M' && g != 'F')
                throw new ArgumentException("Gender must be 'M' or 'F'.");
        }

        private static void ValidateGovernorate(GovernorateId id)
        {
            if (!Enum.IsDefined(typeof(GovernorateId), id))
                throw new ArgumentException("Invalid Governorate ID. Must be 1-5.");
        }
    }
}
