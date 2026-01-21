using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.RequestDTOs
{
    public class CandidateProfileUpdate_RequestDTO
    {
        public string? Goals { get; set; }
        public string? NominationReasons { get; set; }
        public Guid? NationalId { get; set; }

        public Guid? NominationToken { get; set; }
    }
}
