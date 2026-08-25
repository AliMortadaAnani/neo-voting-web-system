using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.Entities
{
    public class EventParticipation
    {
        public int Id { get; private set; }
        public int VoterId { get; private set; }
        public Voter Voter { get; private set; }

        // Support for different event types
        public int? ElectionId { get; private set; }
        public Election? Election { get; private set; }

        public int? PollId { get; private set; }
        public Poll? Poll { get; private set; }

      

     
        private EventParticipation()
        { 
            Election = null!;
            Poll = null!;
            Voter = null!;
        }

        public static EventParticipation CreateForElection(
            int voterId,
            int electionId)
        {
            return new EventParticipation
            {
                VoterId = voterId,
                ElectionId = electionId
            };
        }

        public static EventParticipation CreateForPoll(
            int voterId,
            int pollId)
        {
            return new EventParticipation
            {
                VoterId = voterId,
                PollId = pollId
            };
        }
    }

}
