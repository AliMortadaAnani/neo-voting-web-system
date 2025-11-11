namespace NeoVoting.Domain.Enums
{
    /// <summary>
    /// Defines the types of actions that are recorded in the SystemAuditLog.
    /// </summary>
    public enum SystemActionTypesEnum
    {
        /// <summary>
        /// A user successfully registered as a voter.
        /// </summary>
        VOTER_REGISTERED,

        /// <summary>
        /// A user successfully registered as a candidate.
        /// </summary>
        CANDIDATE_REGISTERED,

        /// <summary>
        /// A candidate profile was successfully created for an election.
        /// </summary>
        CANDIDATE_PROFILE_CREATED
    }
}