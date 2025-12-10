using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.AuthDTOs
{
    public class RegisterVoterDTO
    {
        public Guid? NationalId { get; set; }

        public Guid? VotingToken { get; set; }
    }
}
