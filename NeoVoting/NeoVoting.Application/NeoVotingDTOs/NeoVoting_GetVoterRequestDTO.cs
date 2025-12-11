using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.NeoVotingDTOs
{
    public class NeoVoting_GetVoterRequestDTO
    {
        public Guid? NationalId { get; set; }
        public Guid? VotingToken { get; set; }
    }
}
