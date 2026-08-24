using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.Entities
{
    public class PollAnswer
    {
        public int Id { get; private set; }
        public string Answer { get; private set; } = string.Empty;
        public int PollId { get; private set; }
        public Poll Poll { get; private set; } = null!;

        public ICollection<PollVote> PollVotes { get; private set; } = new List<PollVote>();
        private PollAnswer()
        {
            // Required by EF Core
        }
        public static PollAnswer Create(string answerText, int pollId)
        {
            if (string.IsNullOrWhiteSpace(answerText))
            {
                throw new ArgumentException("Answer text cannot be null or empty.", nameof(answerText));
            }
            return new PollAnswer
            {
                Answer = answerText,
                PollId = pollId
            };
        }
    }
}
