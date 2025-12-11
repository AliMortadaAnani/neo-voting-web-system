using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.NeoVotingDTOs
{
    public class NeoVoting_VoterResponseDTO
    {
        public GovernorateId GovernorateId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public char Gender { get; set; }
        public bool EligibleForElection { get; set; }
        public bool ValidToken { get; set; } // Boolean status only
        public bool IsRegistered { get; set; }
        public bool Voted { get; set; }

    }
}
