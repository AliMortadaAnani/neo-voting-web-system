using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.RequestDTOs
{
    // response dto dont need to be nullable to avoid .Net 8 nullable warnings when validating since they are only used for output
    //otherwise, we need to make every request dto property nullable to avoid the warnings and rely only on FluentValidation for validation

    //We made each property nullable to avoid .Net 8 nullable warnings when validating 
    // we rely only on FluentValidation for validation and to tell us which properties are required
    // we dont want to obtain 2 errors for each property (one from .Net and one from FluentValidation) when a required property is missing
    public class CandidateProfileAdd_RequestDTO
    {
        public string? Goals { get;  set; } 
        public string? NominationReasons { get;  set; }
        public Guid? NationalId { get; set; }

        public Guid? NominationToken { get; set; }

        //Mapping to Domain Entity is in the Service Layer due to needing UserId and ElectionId
    }
}
