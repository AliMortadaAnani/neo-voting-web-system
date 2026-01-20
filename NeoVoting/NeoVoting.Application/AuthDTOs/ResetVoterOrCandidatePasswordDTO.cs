using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.AuthDTOs
{
    public class ResetVoterOrCandidatePasswordDTO
    {
        public string? UserName { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }

        public Guid? NationalId { get; set; }

        public Guid? VotingOrNominationToken { get; set; }
    }
}
