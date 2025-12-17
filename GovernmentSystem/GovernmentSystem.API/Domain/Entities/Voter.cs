using GovernmentSystem.API.Domain.Shared;

namespace GovernmentSystem.API.Domain.Entities
{
    public class Voter
    {
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

        //Private Constructor (EF Core requires this or a binding constructor)
        private Voter()
        { }

        //Static Create Method (Factory Method)
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
            ValidateAge(dateOfBirth);
            ValidateNames(firstName, lastName);
            return new Voter
            {
                Id = Guid.NewGuid(),           // Generated automatically
                NationalId = Guid.NewGuid(),   // Generated automatically (No available citizens datbase so we need to provide an id on our own)
                VotingToken = Guid.NewGuid(),  // Generated automatically
                GovernorateId = governorateId,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Gender = char.ToUpper(gender),
                EligibleForElection = eligibleForElection,
                ValidToken = true,             // Default to true on creation
                IsRegistered = false,          // Default to false on creation
                Voted = false                  // Default to false on creation
            };
        }

        //Update Method (All fields except Id, nationalId and VotingToken)
        //Search for record by nationalId and then call this method to update other details
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
            ValidateAge(dateOfBirth);
            ValidateNames(firstName, lastName);
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

        //Set New Token Method for governmental and security purposes based on legal requests
        public void GenerateNewVotingToken()
        {
            VotingToken = Guid.NewGuid();
            ValidToken = true; // Re-enable token if it was invalid
        }

        public void MarkVoterAsRegistered() //Initially called by NeoVoting system when registering the voter (voter registration is system wide, not per election)
        {
            //Should not arrive here if well handled in the service layer
            if (!ValidToken || !EligibleForElection || IsRegistered)
            {
                throw new InvalidOperationException("Cannot register voter with invalid token or ineligible for election or already registered.");
                //we are not accepting re-registration requests because that could be a sign of malicious activity
            }
            IsRegistered = true;
        }

        public void MarkVoterAsVoted() //Initially called by NeoVoting system when voter is casting his vote (casitng a vote is not related to a specific election)
                                       // we rely on Voted flag to determine if the voter has voted
                                       // in the current election or not
                                       // we always reset the Voted flag after each election
        {
            //Should not arrive here if well handled in the service layer
            if (!IsRegistered || !ValidToken || !EligibleForElection || Voted)
            {
                throw new InvalidOperationException("Voter cannot vote with invalid token or ineligible for election or unregistered or already voted in this election");
                //we are not accepting voting requests if Voted flag is true because that could be a sign of malicious activity and to prevent double voting
            }
            Voted = true;
        }

        //Helpers to sanitize and validate inputs(last resort, should be handled in validation layer ideally by Fluent Validators)
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
            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year - dateOfBirth.Year;

            // Adjust age if the birthday hasn't occurred yet this year
            if (dateOfBirth > today.AddYears(-age))
            {
                age--;
            }

            if (age < 18) // Neo-Voting Lebanese voting age
            {
                throw new ArgumentException("Person must be at least 18 years old.");
            }
        }
    }
}