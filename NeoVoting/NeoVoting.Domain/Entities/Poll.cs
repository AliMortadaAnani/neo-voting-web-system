using NeoVoting.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.Entities
{
    public class Poll
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public StatusEnum Status { get; private set; }

        public ICollection<PollAnswer> PollAnswers { get; private set; } = new List<PollAnswer>();

        public ICollection<PollVote> PollVotes { get; private set; } = new List<PollVote>();

        public ICollection<EventParticipation> EventParticipations { get; private set; } = new List<EventParticipation>();
        private Poll()
        {
            // Required by EF Core
        }
       
        
        public static Poll Create(string name, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Poll name cannot be null or empty.", nameof(name));
            }
            if (startDate >= endDate)
            {
                throw new ArgumentException("Start date must be earlier than end date.");
            }
            return new Poll
            {
                Name = name,
                StartDate = startDate,
                EndDate = endDate
            };
        }
    }
}
