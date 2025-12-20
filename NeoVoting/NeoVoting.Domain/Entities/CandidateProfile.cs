using NeoVoting.Domain.IdentityEntities;
using System.Text;

namespace NeoVoting.Domain.Entities
{
    public class CandidateProfile
    {
        public Guid Id { get; private set; }
        public string Goals { get; private set; } = string.Empty;
        public string NominationReasons { get; private set; } = string.Empty;
        public string? ProfilePhotoFilename { get; private set; }

        // --- Foreign Keys & Navigation Properties ---

        public Guid UserId { get; private set; } // The user who is the candidate
        public ApplicationUser User { get; private set; }

        public Guid ElectionId { get; private set; } // The election they are running in

        //same user as candidate can have multiple profiles in different elections (one profile per election to be considered nominated for that election)
        public Election Election { get; private set; }

        private CandidateProfile()
        {
            User = null!;
            Election = null!;
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
            ValidateCreation(userId, electionId, goals, nominationReasons);

            var profile = new CandidateProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ElectionId = electionId,
                Goals = goals,
                NominationReasons = nominationReasons,
                ProfilePhotoFilename = profilePhotoUrl // A photo is not required at creation and is managed separately.
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
            ValidateUpdate(goals, nominationReasons);

            Goals = goals;
            NominationReasons = nominationReasons;
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
            ProfilePhotoFilename = photoUrl;
        }

        /// <summary>
        /// Removes the profile photo URL from the candidate's profile.
        /// Note: This does not delete the physical file, which should be handled by the application layer.
        /// </summary>
        public void RemoveProfilePhoto()
        {
            ProfilePhotoFilename = null;
        }

        // --- Private Validation Helper ---

        /// <summary>
        /// Private helper method to contain all validation rules for profile text fields.
        /// </summary>
        private static void ValidateCreation(Guid userId, Guid electionId, string goals, string nominationReasons)
        {
            var errors = new StringBuilder();

            if (userId == Guid.Empty)
            {
                errors.AppendLine("UserId is required for the creation of candidate profile.");
            }
            if (electionId == Guid.Empty)
            {
                errors.AppendLine("ElectionId is required for the creation of candidate profile.");
            }
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

        private static void ValidateUpdate(string goals, string nominationReasons)
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