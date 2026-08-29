using NeoVoting.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ResponseDTOs.GeneralDTOs
{
    public class CandidateResults_ResponseDTO
    {
        public int? CandidateProfileId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; } 

        public GovernorateIdEnum? Governorate { get; set; }
        public string? ProfilePhotoFilename { get; set; }
        public int? VoteCount { get; set; }
    }
}
