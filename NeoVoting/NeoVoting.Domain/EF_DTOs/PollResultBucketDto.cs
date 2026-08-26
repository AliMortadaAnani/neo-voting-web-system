using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.EF_DTOs
{
    public class PollResultBucketDto
    {
        public required PollAnswer Answer { get; set; }
        public int VoteCount { get; set; }
    }
}
