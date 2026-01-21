using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ResponseDTOs
{
    // response dto dont need to be nullable to avoid .Net 8 nullable warnings when validating since they are only used for output
    //otherwise, we need to make every request dto property nullable to avoid the warnings and rely only on FluentValidation for validation
    public class CandidateProfile_ResponseDTO
    {
    }
}
