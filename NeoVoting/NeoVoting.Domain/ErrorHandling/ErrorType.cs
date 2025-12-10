using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.ErrorHandling
{
    public enum ErrorType
    {
        Failure = 0,      // Generic / unexpected domain/infra failure → usually 500
        Validation = 1,   // 400 Bad Request // dealt with FluentValidation
        NotFound = 2,     // 404 Not Found
        Conflict = 3,     // 409 Already exists / conflict
        Forbidden = 4,    // 403 Not allowed but authenticated
        Unauthorized = 5, // 401 (optional but useful) not authenticated
        None = 6          // Represents "no error"
    }
}
