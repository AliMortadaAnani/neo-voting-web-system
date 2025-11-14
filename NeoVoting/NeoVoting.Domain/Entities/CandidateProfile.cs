using NeoVoting.Domain.IdentityEntities;
using System.Text;

namespace NeoVoting.Domain.Entities
{
    public class CandidateProfile
    {
        // --- Properties ---

        public Guid Id { get; private set; }
        public string Goals { get; set; } = string.Empty;
        public string NominationReasons { get; set; } = string.Empty;
        public string? ProfilePhotoUrl { get; set; }

        // --- Foreign Keys & Navigation Properties ---

        public Guid UserId { get; private set; } // The user who is the candidate
        public ApplicationUser User { get; private set; }

        public Guid ElectionId { get; private set; } // The election they are running in
        public Election Election { get; private set; }


        // --- Constructor ---

        /// <summary>
        /// A private constructor to force all object creation to go through the
        /// controlled, static factory method. EF Core uses this for materializing.
        /// </summary>
        private CandidateProfile()
        {
            User = null!;
            Election = null!;
        }


        // --- ToString() Override ---

        /// <summary>
        /// Provides a detailed, multi-line string representation of the candidate profile,
        /// which is extremely useful for debugging and logging.
        /// </summary>
        /// <returns>A comprehensive string summary of the profile.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CandidateProfile (Id: {Id})");
            sb.AppendLine($"  For User ID: {UserId}");
            sb.AppendLine($"  In Election ID: {ElectionId}");
            sb.AppendLine($"  Goals: {Goals.Substring(0, Math.Min(Goals.Length, 70))}...");
            sb.AppendLine($"  Photo URL: {(string.IsNullOrEmpty(ProfilePhotoUrl) ? "None" : ProfilePhotoUrl)}");
            return sb.ToString();
        }


        // --- Factory Method ---

        /// <summary>
        /// Creates a new, valid CandidateProfile instance.
        /// This represents a user's official nomination for a specific election.
        /// </summary>
        /// <param name="userId">The ID of the user becoming a candidate.</param>
        /// <param name="electionId">The ID of the election they are running in.</param>
        /// <param name="goals">The candidate's stated goals.</param>
        /// <param name="nominationReasons">The reasons the candidate is running.</param>
        /// <returns>A new, valid CandidateProfile object.</returns>
        /// <exception cref="ArgumentException">Thrown if validation of text inputs fails.</exception>
        public static CandidateProfile Create(Guid userId, Guid electionId, string goals, string nominationReasons, string? profilePhotoUrl = null)
        {
            // --- Centralized Validation Logic ---
            Validate(goals, nominationReasons);

            var profile = new CandidateProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ElectionId = electionId,
                Goals = goals,
                NominationReasons = nominationReasons,
                ProfilePhotoUrl = profilePhotoUrl // A photo is not required at creation and is managed separately.
            };

            return profile;
        }

        // --- Instance Methods ---

        /// <summary>
        /// Updates the mutable text fields of the candidate's profile.
        /// </summary>
        /// <param name="goals">The updated goals for the candidate.</param>
        /// <param name="nominationReasons">The updated reasons for nomination.</param>
        public void UpdateDetails(string goals, string nominationReasons)
        {
            // --- Re-use the same validation logic ---
            Validate(goals, nominationReasons);

            this.Goals = goals;
            this.NominationReasons = nominationReasons;
        }

        /// <summary>
        /// Sets or updates the profile photo URL for the candidate.
        /// </summary>
        /// <param name="photoUrl">The unique filename (e.g., "guid.jpg") of the uploaded photo.</param>
        public void SetProfilePhoto(string photoUrl)
        {
            if (string.IsNullOrWhiteSpace(photoUrl))
            {
                throw new ArgumentException("Profile photo URL cannot be empty.", nameof(photoUrl));
            }
            this.ProfilePhotoUrl = photoUrl;
        }

        /// <summary>
        /// Removes the profile photo URL from the candidate's profile.
        /// Note: This does not delete the physical file, which should be handled by the application layer.
        /// </summary>
        public void RemoveProfilePhoto()
        {
            this.ProfilePhotoUrl = null;
        }


        // --- Private Validation Helper ---

        /// <summary>
        /// Private helper method to contain all validation rules for profile text fields.
        /// </summary>
        private static void Validate(string goals, string nominationReasons)
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(goals))
            {
                errors.AppendLine("Candidate goals are required.");
            }

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