using System.ComponentModel;

namespace NeoVoting.Domain.Enums
{
    public enum ElectionStatusEnum
    {
        [Description("Upcoming")]
        Upcoming = 1,

        [Description("Nomination")]
        Nomination = 2,

        [Description("Voting")]
        Voting = 3,

        [Description("Completed")]
        Completed = 4
    }
}