using GovernmentSystem.API.Domain.Enums;

namespace GovernmentSystem.API.Domain.Entities
{
    public class Citizen
    {
        public int Id { get; private set; } // auto-incremented by DB
        public string NationalId { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public DateOnly DateOfBirth { get; private set; }
        public GovernorateIdEnum GovernorateId { get; private set; }
        public char Gender { get; private set; } // 'M'/ 'm' or 'F' / 'f'

        private Citizen()
        { }

        public static Citizen Create(
            string nationalId,
            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            char gender,
            GovernorateIdEnum governorateId
            )
        {
            ValidateNationalId(nationalId);
            ValidateNames(firstName, lastName);
            ValidateAge(dateOfBirth);
            ValidateGender(gender);
            ValidateGovernorate(governorateId);
            return new Citizen
            {
                NationalId = nationalId,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Gender = char.ToUpper(gender),
                GovernorateId = governorateId
            };
        }

        //Update Method (All fields except Id, nationalId)
        //Search for record by nationalId and then call this method to update other details
        public void Update(
            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            char gender,
            GovernorateIdEnum governorateId)
        {
            ValidateNames(firstName, lastName);
            ValidateAge(dateOfBirth);
            ValidateGender(gender);
            ValidateGovernorate(governorateId);
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = char.ToUpper(gender);
            GovernorateId = governorateId;
        }

        //Helpers to sanitize and validate inputs(last resort, should be handled in validation layer ideally by Fluent Validators)

        private static void ValidateNationalId(string nationalId)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
                throw new ArgumentException("National ID must not be null, empty, or whitespace.", nameof(nationalId));
        }

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

        private static void ValidateGovernorate(GovernorateIdEnum id)
        {
            if (!Enum.IsDefined(typeof(GovernorateIdEnum), id))
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

            if (age < 18) // Neo-Voting Lebanese voting age
            {
                throw new ArgumentException("Person must be at least 18 years old.");
            }
        }
    }
}