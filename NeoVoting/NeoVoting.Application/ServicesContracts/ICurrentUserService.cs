using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ServicesContracts
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        // int? UserId { get; } // Use int if your IDs are integers
    }
}
