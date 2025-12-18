using System.ComponentModel;

namespace NeoVoting.Domain.Enums
{
    public enum ElectionStatusEnum
    {
        [Description("Upcoming")]
        Upcoming = 1,

        [Description("Nomination")]
        Nomination = 2,

        [Description("Pre-Voting Phase")]
        PreVotingPhase = 3,

        [Description("Voting")]
        Voting = 4,

        [Description("Completed")]
        Completed = 5
    }
}