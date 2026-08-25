using Microsoft.AspNetCore.Identity;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using System.Text;

namespace NeoVoting.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<int>
    {
        // For JWT Refresh Tokens
        public string? RefreshToken { get; private set; }

        public DateTime? RefreshTokenExpirationDateTime { get; private set; }

        public Candidate? Candidate { get; private set; }

        public Voter? Voter { get; private set; }
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