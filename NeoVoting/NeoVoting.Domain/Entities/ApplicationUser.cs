using Microsoft.AspNetCore.Identity;
using System.Text;

namespace NeoVoting.Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        // For JWT Refresh Tokens
        public string? RefreshToken { get; private set; }

        public DateTime? RefreshTokenExpirationDateTime { get; private set; }

        public Candidate? Candidate { get; private set; } // Candidate IS a user, but not all users are candidates. This is a one-to-one relationship.

        public Voter? Voter { get; private set; } // Voter IS a user, but not all users are voters. This is a one-to-one relationship.

        // admin do not have a separate entity, they are just users with a specific role. So no need for an Admin entity.

        private ApplicationUser()
        { }

        public static ApplicationUser CreateAccount(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Username is required for an creating an account.", nameof(userName));
            }

            var user = new ApplicationUser
            {
                UserName = userName,
            };

            return user;
        }

        // --- Token Management Methods ---

        public void UpdateRefreshToken(string token, DateTime expiryDateTime)
        {
            ValidateRefreshTokenParams(token, expiryDateTime);

            RefreshToken = token;
            RefreshTokenExpirationDateTime = expiryDateTime;
        }

        public void InvalidateRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpirationDateTime = DateTime.UtcNow.AddMinutes(-1); ;
        }

        private static void ValidateRefreshTokenParams(string token, DateTime expiryDateTime)
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(token))
            {
                errors.AppendLine("Refresh token cannot be null or empty.");
            }

            // Ensure the expiry is in the future.

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