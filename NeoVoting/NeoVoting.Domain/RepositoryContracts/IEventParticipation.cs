using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IEventParticipation
    {
        void Add(EventParticipation participation);

        Task<bool> HasVoterVotedByVoterIdAndElectionIdAsync(int voterId, int electionId);
        Task<bool> HasVoterVotedByVoterIdAndPollIdAsync(int voterId, int pollId);

    }
}
