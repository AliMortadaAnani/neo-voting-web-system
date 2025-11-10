using System.ComponentModel;

namespace NeoVoting.Domain.Enums
{
    public enum GovernoratesEnum
    {
        [Description("Beirut")]
        Beirut = 1,

        [Description("Mount Lebanon")]
        MountLebanon = 2,

        [Description("South")]
        South = 3,

        [Description("East")]
        East = 4,

        [Description("North")]
        North = 5
    }
}