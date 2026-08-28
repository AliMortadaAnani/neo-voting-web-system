using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.EF_DTOs
{
    public class PollResultBucketDto
    {
        public required PollAnswer Answer { get; set; }
        public int VoteCount { get; set; }
    }
}
