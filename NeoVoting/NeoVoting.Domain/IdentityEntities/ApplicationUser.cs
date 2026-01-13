using Microsoft.AspNetCore.Identity;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using System.Text;

namespace NeoVoting.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        // For JWT Refresh Tokens
        public string? RefreshToken { get; private set; }

        public DateTime? RefreshTokenExpirationDateTime { get; private set; }

        // For personal details (nullable to support admin accounts)
        // these properties will be populated only for Voter and Candidate accounts
        // and only from the GovernmentSystem api during account registration in NeoVoting.
        public string? FirstName { get; private set; }

        public string? LastName { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public char? Gender { get; private set; }
        public int? GovernorateId { get; private set; }

        public Governorate? Governorate { get; private set; } // Navigation property

        // --- Constructor ---

        /// <summary>
        /// A private constructor is used to prevent direct instantiation with 'new'.
        /// This forces all object creation to go through the controlled factory methods.
        /// Entity Framework Core is smart enough to use this for materializing objects from the DB.
        /// </summary>
        private ApplicationUser()
        { }

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
                GovernorateId = governorateId
            };

            return participantUser;
        }

        /// <summary>
        /// Private helper method to contain the validation rules for participants.
        /// </summary>
        //Note that these properties are already validated from GovernmentSystem
        //we are just filling them in our User entity
        //Validation is done here to ensure data integrity
        private static void ValidateParticipantDetails(string userName, string firstName, string lastName, DateTime dateOfBirth, char gender, int governorateId)
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(userName)) errors.AppendLine("Username is required.");

            if (string.IsNullOrWhiteSpace(firstName)) errors.AppendLine("First name is required for user.");

            if (string.IsNullOrWhiteSpace(lastName)) errors.AppendLine("Last name is required for user.");

            if (!IsAtLeast18YearsOld(dateOfBirth))
            {
                errors.AppendLine("User must be at least 18 years old.");
            }

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

        //helper method to check age
        private static bool IsAtLeast18YearsOld(DateTime dateOfBirth, DateTime? asOfDate = null)
        {
            var today = (asOfDate ?? DateTime.UtcNow).Date;
            var age = today.Year - dateOfBirth.Year;
            // Adjust if birthday hasn't occurred yet this year
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age >= 18;
        }

        // --- Token Management Methods ---

        /// <summary>
        /// Updates the user's refresh token and its expiration date.
        /// Use this when a user logs in or refreshes their session.
        /// </summary>
        /// <param name="token">The new cryptographic refresh token string.</param>
        /// <param name="expiryDateTime">The UTC date and time when this token expires.</param>
        public void UpdateRefreshToken(string token, DateTime expiryDateTime)
        {
            ValidateRefreshTokenParams(token, expiryDateTime);

            RefreshToken = token;
            RefreshTokenExpirationDateTime = expiryDateTime;
        }

        /// <summary>
        /// Revokes the current refresh token by setting it and its expiration to null.
        /// Use this when a user logs out or if a security breach is suspected.
        /// </summary>
        public void InvalidateRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpirationDateTime = DateTime.UtcNow.AddMinutes(-1); ;
        }

        /// <summary>
        /// Helper method to validate parameters for setting a refresh token.
        /// </summary>
        private static void ValidateRefreshTokenParams(string token, DateTime expiryDateTime)
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(token))
            {
                errors.AppendLine("Refresh token cannot be null or empty.");
            }

            // Ensure the expiry is in the future.
            // Depending on your logic, you might want a buffer (e.g., > 1 minute from now),
            // but strict > UtcNow is the baseline requirement.
            if (expiryDateTime <= DateTime.UtcNow)
            {
                errors.AppendLine($"Refresh token expiration must be in the future. Provided: {expiryDateTime} UTC, Now: {DateTime.UtcNow} UTC.");
            }

            if (errors.Length > 0)
            {
                throw new ArgumentException(errors.ToString());
            }
        }
    }
}