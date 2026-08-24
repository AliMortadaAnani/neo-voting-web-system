namespace NeoVoting.Domain.Enums
{
    /// <summary>
    /// Defines the types of actions that are recorded in the SystemAuditLog.
    /// </summary>
    public enum SystemActionTypesEnum
    {
        ADMIN_CREATED_ELECTION = 1,
        ADMIN_STARTED_VOTING_PHASE = 2,
        ADMIN_ENDED_VOTING_PHASE = 3,

        ADMIN_CREATED_POLL = 4,
        ADMIN_STARTED_POLL = 5,
        ADMIN_ENDED_POLL = 6,

        ADMIN_BANNED_CANDIDATE_ACCOUNT = 7,

        ADMIN_BANNED_VOTER_ACCOUNT = 8
    }
}