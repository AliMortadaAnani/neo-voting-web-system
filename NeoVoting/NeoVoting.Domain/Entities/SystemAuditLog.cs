using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.Entities
{
    public class SystemAuditLog
    {
        public long Id { get; private set; }
        public DateTime TimestampUTC { get; private set; }
        public SystemActionTypesEnum ActionType { get; private set; }
        public string? Details { get; private set; }
        public Guid UserId { get; private set; }

        // Snapshots
        public string Username { get; private set; } = string.Empty;
        public string? ElectionName { get; private set; }

        // Links
        public Guid? CandidateProfileId { get; private set; }
        public Guid? ElectionId { get; private set; }

        private SystemAuditLog() { }

        public static SystemAuditLog Create(
            Guid userId,
            string userName,
            SystemActionTypesEnum actionType,
            string? details,
            Guid? candidateProfileId,
            Guid? electionId,
            string? electionName) // Made nullable here to allow other actions to pass null
        {
            Validate(userId, userName, actionType);

            // BUSINESS RULE: Candidate Creation requires linking to an Election
            if (actionType == SystemActionTypesEnum.CANDIDATE_PROFILE_CREATED)
            {
                if (candidateProfileId == null)
                    throw new ArgumentException("CandidateProfileId is required for CANDIDATE_PROFILE_CREATED.");

                if (electionId == null)
                    throw new ArgumentException("ElectionId is required for CANDIDATE_PROFILE_CREATED.");

                // ADDED: Enforce the Snapshot Name too!
                // If we have an ID, we MUST have the name for historical truth.
                if (string.IsNullOrWhiteSpace(electionName))
                    throw new ArgumentException("ElectionName (Snapshot) is required when ElectionId is present.");
            }

            return new SystemAuditLog
            {
                UserId = userId,
                Username = userName,
                ActionType = actionType,
                Details = details,
                CandidateProfileId = candidateProfileId,
                ElectionId = electionId,
                ElectionName = electionName,
                TimestampUTC = DateTime.UtcNow
            };
        }

        private static void Validate(Guid userId, string userName, SystemActionTypesEnum actionType)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required.", nameof(userId));

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Username is required.", nameof(userName));

            if (!Enum.IsDefined(typeof(SystemActionTypesEnum), actionType))
                throw new ArgumentException("Valid ActionType is required.", nameof(actionType));
        }
    }
}