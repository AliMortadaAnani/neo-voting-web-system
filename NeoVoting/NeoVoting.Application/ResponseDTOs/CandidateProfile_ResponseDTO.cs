using NeoVoting.Domain.Entities;
using NeoVoting.Domain.IdentityEntities;
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
        public Guid Id { get;  set; }
        public string? Goals { get;  set; } 
        public string? NominationReasons { get; set; } 
        public string? ProfilePhotoFilename { get; set; }

        public Guid UserId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public char? Gender { get; set; }
        public int? GovernorateId { get; set; }

        public Guid ElectionId { get; set; }

     
    }
}
