using NeoVoting.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.Entities
{
    public class PublicPollLog
    {
        public long Id { get; private set; }
        public Guid PollVoteId { get; private set; }
        public DateTime TimestampUTC { get; private set; }
        public int PollId { get; private set; }
        public string PollName { get; private set; } = string.Empty;
       

        private PublicPollLog() { }

        // --- Factory Method ---

        /// <summary>
        /// Creates a new PublicPollLog instance to record the occurrence of a poll vote.
        /// </summary>
        public static PublicPollLog Create(
            Guid pollVoteId,
            int pollId,
     
            string pollName
            )
        {
            return new PublicPollLog
            {
                PollVoteId = pollVoteId,
                PollId = pollId,
                PollName = pollName,
         
                TimestampUTC = DateTime.UtcNow
            };
        }

    }
}
