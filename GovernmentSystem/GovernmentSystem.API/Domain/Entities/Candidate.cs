using GovernmentSystem.API.Domain.Shared;
using System.Text;

namespace GovernmentSystem.API.Domain.Entities
{
    public class Candidate
    {
        // 1. Properties
        public Guid Id { get; private set; }
        public Guid NationalId { get; private set; }
        public Guid NominationToken { get; private set; }
        public GovernorateId GovernorateId { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public DateOnly DateOfBirth { get; private set; }
        public char Gender { get; private set; }
        public bool EligibleForElection { get; private set; }
        public bool ValidToken { get; private set; }
        public bool IsRegistered { get; private set; }

        // Note: Candidates don't have a "Voted" field in this schema 
        // (they vote using their Voter record)

        // 2. Private Constructor
        private Candidate() { }

        // 3. Static Create Method
        public static Candidate Create(
            GovernorateId governorateId,
            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            char gender,
            bool eligibleForElection)
        {
            ValidateGender(gender);
            ValidateGovernorate(governorateId);

            return new Candidate
            {
                Id = Guid.NewGuid(),
                NationalId = Guid.NewGuid(),
                NominationToken = Guid.NewGuid(),
                GovernorateId = governorateId,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Gender = char.ToUpper(gender),
                EligibleForElection = eligibleForElection,
                ValidToken = true,
                IsRegistered = false
            };
        }

        // 4. Update Method (All fields except Id and NominationToken)
        public void UpdateDetails(
            
            GovernorateId governorateId,
            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            char gender,
            bool eligibleForElection,
            bool validToken,
            bool isRegistered)
        {
            ValidateGender(gender);
            ValidateGovernorate(governorateId);
            ValidateAge(dateOfBirth);

            GovernorateId = governorateId;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = char.ToUpper(gender);
            EligibleForElection = eligibleForElection;
            ValidToken = validToken;
            IsRegistered = isRegistered;
        }

        // 5. Set New Token Method
        public void GenerateNewNominationToken()
        {
            NominationToken = Guid.NewGuid();
            ValidToken = true;
        }
        public void MarkCandidateAsRegistered()
        {
            IsRegistered = true;
        }
        // 6. ToString Method
        public override string ToString()
        {
            return new StringBuilder()
                .AppendLine($"[Candidate Record]")
                .AppendLine($"Id: {Id}")
                .AppendLine($"NationalID: {NationalId}")
                .AppendLine($"NominationToken: {NominationToken}")
                .AppendLine($"Governorate: {GovernorateId}")
                .AppendLine($"Name: {FirstName} {LastName}")
                .AppendLine($"DOB: {DateOfBirth}")
                .AppendLine($"Gender: {Gender}")
                .AppendLine($"Eligible: {EligibleForElection}")
                .AppendLine($"ValidToken: {ValidToken}")
                .AppendLine($"Registered: {IsRegistered}")
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
        private static void ValidateAge(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year - dateOfBirth.Year;

            // Adjust age if the birthday hasn't occurred yet this year
            if (dateOfBirth > today.AddYears(-age))
            {
                age--;
            }

            if (age < 18) // Lebanese voting age
            {
                throw new ArgumentException("Person must be at least 18 years old.");
            }
        }

    }
}
