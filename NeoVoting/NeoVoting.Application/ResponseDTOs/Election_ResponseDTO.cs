using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Application.ResponseDTOs
{
    public class Election_ResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get;  set; } = string.Empty;
        public DateTime NominationStartDate { get; set; }
        public DateTime NominationEndDate { get; set; }
        public DateTime VotingStartDate { get; set; }
        public DateTime VotingEndDate { get; set; }

        public int ElectionStatusId { get; set; }
        public string ElectionStatusName { get; set; } = string.Empty;
    }
}
