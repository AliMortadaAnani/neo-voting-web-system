using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ServicesContracts
{
    public interface ICurrentUserServices
    {
        Guid? UserId { get; }
        Guid GetAuthenticatedUserId();
      
    }
}
