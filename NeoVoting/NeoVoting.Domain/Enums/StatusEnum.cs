using System.ComponentModel;

namespace NeoVoting.Domain.Enums
{
    public enum StatusEnum
    {
        [Description("Upcoming")]
        Upcoming = 1,

        [Description("Voting")]
        Voting = 2,

        [Description("Completed")]
        Completed = 3
    }
}