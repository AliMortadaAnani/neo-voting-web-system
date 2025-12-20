using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Domain.Entities
{
    public class Candidate
    {
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

        public string? RegisteredUsername { get; private set; }

        // Note: Candidates don't have a "Voted" field in this schema
        // (they vote using their Voter record here - Voter account in NeoVoting)

        private Candidate()
        { }

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
            ValidateAge(dateOfBirth);
            ValidateNames(firstName, lastName);
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
                IsRegistered = false,
                RegisteredUsername = null
            };
        }

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
            ValidateNames(firstName, lastName);
            GovernorateId = governorateId;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = char.ToUpper(gender);
            EligibleForElection = eligibleForElection;
            ValidToken = validToken;
            IsRegistered = isRegistered;// We allow updating IsRegistered flag in case of manual corrections by admin when critical issues arise
        }

        public void GenerateNewNominationToken()
        {
            NominationToken = Guid.NewGuid();
            ValidToken = true;
        }

        public void MarkCandidateAsRegisteredWithNewRegisteredUsername(string registeredUsername)
        {   
            if(string.IsNullOrWhiteSpace(registeredUsername))
            {
                throw new ArgumentException("Registered username must not be null, empty, or whitespace.", nameof(registeredUsername));
            }
            if (!ValidToken || !EligibleForElection || IsRegistered)
            {
                throw new InvalidOperationException("Cannot register candidate with invalid token or ineligible for election or already registered.");
            }

            IsRegistered = true;
            RegisteredUsername = registeredUsername;
        }

        //Helpers
        private static void ValidateNames(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name must not be null, empty, or whitespace.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name must not be null, empty, or whitespace.", nameof(lastName));
        }

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
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - dateOfBirth.Year;

            // Adjust age if the birthday hasn't occurred yet this year
            if (dateOfBirth > today.AddYears(-age))
            {
                age--;
            }

            if (age < 18) // Neo-Voting Lebanese nomination age
            {
                throw new ArgumentException("Person must be at least 18 years old.");
            }
        }

        //Ado.Net Materialization Method
        //We tried to communicate with Database using ADO.Net directly for performance reasons
        //this method is used to fill the Candidate entity from the data retrieved from the database
        public static Candidate FromAdoNet(
    Guid id,
    Guid nationalId,
    Guid nominationToken,
    GovernorateId governorateId,
    string firstName,
    string lastName,
    DateOnly dateOfBirth,
    char gender,
    bool eligibleForElection,
    bool validToken,
    bool isRegistered,
    string? registeredUsername
            )
        {
            return new Candidate
            {
                Id = id,
                NationalId = nationalId,
                NominationToken = nominationToken,
                GovernorateId = governorateId,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Gender = char.ToUpper(gender),
                EligibleForElection = eligibleForElection,
                ValidToken = validToken,
                IsRegistered = isRegistered,
                RegisteredUsername = registeredUsername
            };
        }
    }
}