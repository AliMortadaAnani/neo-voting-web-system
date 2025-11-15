using NeoVoting.Domain.Entities;

namespace NeoVoting.Application.RequestDTOs
{
    /// <summary>
    /// Represents the data required to create a new candidate profile's text-based details.
    /// The profile photo is handled in a separate request.
    /// </summary>
    public class CandidateProfileAddRequest
    {
        public Guid UserId { get; set; }
        public Guid ElectionId { get; set; }
        public string Goals { get; set; } = string.Empty;
        public string NominationReasons { get; set; } = string.Empty;


        //

        /// <summary>
        /// Maps this DTO to a new CandidateProfile domain entity.
        /// This is where the initial creation logic is centralized.
        /// </summary>
        /// <returns>A new, validated CandidateProfile entity.</returns>
        public CandidateProfile ToCandidateProfile()
        {
            // We use the domain entity's factory method to ensure all business rules are enforced.
            return CandidateProfile.Create(UserId, ElectionId, Goals, NominationReasons);
        }
    }
}

