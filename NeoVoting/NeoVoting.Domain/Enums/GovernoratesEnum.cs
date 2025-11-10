using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.Enums
{
    public enum GovernoratesEnum
    {
        // The names here are what you'll use in your C# code.
        // The numbers are what get stored in the database.
        Beirut = 1,
        MountLebanon = 2, // Use PascalCase for enum members
        South = 3,
        East = 4,
        North = 5
    }
}
