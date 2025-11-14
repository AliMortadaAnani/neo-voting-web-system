using Microsoft.AspNetCore.Identity;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using System.Text;

namespace NeoVoting.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        // For JWT Refresh Tokens
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpirationDateTime { get; set; }

        // For personal details (nullable to support admin accounts)
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public char? Gender { get; set; }
        public int? GovernorateID { get; private set; }

        public Governorate? Governorate { get; private set; } // Navigation property


        /// <summary>
        /// Overrides the default ToString() method to provide a detailed, single-line
        /// representation of the user object. This is extremely useful for debugging and logging.
        /// It safely handles nullable properties.
        /// </summary>
        /// <returns>A comprehensive string summary of the user.</returns>
        public override string ToString()
        {
            // StringBuilder is efficient for building strings from multiple parts.
            var sb = new StringBuilder();

            // Start with the most important, non-nullable identifiers.
            sb.Append($"User: {UserName} (Id: {Id})");

            // Append personal details only if they exist.
            if (!string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName))
            {
                sb.Append($", Name: {FirstName} {LastName}");
            }

            if (DateOfBirth.HasValue)
            {
                // Using a standard, unambiguous date format.
                sb.Append($", DOB: {DateOfBirth.Value.ToString("yyyy-MM-dd")}");
            }

            if (Gender.HasValue)
            {
                sb.Append($", Gender: {Gender.Value}");
            }

            if (GovernorateID.HasValue)
            {
                sb.Append($", GovID: ({GovernorateID.Value})" +
                    $" {((GovernoratesEnum)GovernorateID).GetDescription()}");
            }

            // For the refresh token, just indicate its presence, not the token itself.
            // This is cleaner and more secure.
            sb.Append($", RefreshToken: {(string.IsNullOrEmpty(RefreshToken) ? "None" : "Present")}");

            if (RefreshTokenExpirationDateTime.HasValue)
            {
                sb.Append($", Expires: {RefreshTokenExpirationDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")} UTC");
            }

            return sb.ToString();
        }



        // --- Constructor ---

        /// <summary>
        /// A private constructor is used to prevent direct instantiation with 'new'.
        /// This forces all object creation to go through the controlled factory methods.
        /// Entity Framework Core is smart enough to use this for materializing objects from the DB.
        /// </summary>
        private ApplicationUser() { }


        // --- Factory Methods ---

        /// <summary>
        /// Factory for creating a system Administrator.
        /// This logic path only requires a username. Personal details are left null.
        /// The password is NOT handled here; it's handled by UserManager.
        /// Id assigned here.
        /// </summary>
        public static ApplicationUser CreateAdminAccount(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Username is required for an admin account.", nameof(userName));
            }

            var adminUser = new ApplicationUser
            {

                // The Id is generated here, making the object complete.
                Id = Guid.NewGuid(),
                UserName = userName,

            };

            return adminUser;
        }

        /// <summary>
        /// Factory for creating a participant (Voter or Candidate).
        /// This logic path requires all personal details to be provided and valid.
        /// The password is NOT handled here; it's handled by UserManager.
        /// Id assigned here.
        /// </summary>
        public static ApplicationUser CreateVoterOrCandidateAccount(string userName, string firstName, string lastName, DateTime dateOfBirth, char gender, int governorateId)
        {
            // --- Validation logic is centralized right here ---
            ValidateParticipantDetails(userName, firstName, lastName, dateOfBirth, gender, governorateId);

            var genderUpper = char.ToUpper(gender);

            var participantUser = new ApplicationUser
            {
                // The Id is generated here, making the object complete.
                Id = Guid.NewGuid(),
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Gender = genderUpper,
                GovernorateID = governorateId
            };

            return participantUser;
        }

        /// <summary>
        /// Private helper method to contain the validation rules for participants.
        /// </summary>
        private static void ValidateParticipantDetails(string userName, string firstName, string lastName, DateTime dateOfBirth, char gender, int governorateId)
        {
            var errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(userName)) errors.AppendLine("Username is required.");
            if (string.IsNullOrWhiteSpace(firstName)) errors.AppendLine("First name is required for participants.");
            if (string.IsNullOrWhiteSpace(lastName)) errors.AppendLine("Last name is required for participants.");
            if (dateOfBirth > DateTime.UtcNow.AddYears(-18)) errors.AppendLine("Participant must be at least 18 years old.");
            if (char.ToUpper(gender) != 'M' && char.ToUpper(gender) != 'F') errors.AppendLine("Gender must be 'M' or 'F'.");

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
