using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IEventParticipationRepository
    {
        void Add(EventParticipation participation);

        Task<bool> HasVoterVotedByVoterIdAndElectionIdAsync(int voterId, int electionId);

        Task<bool> HasVoterVotedByVoterIdAndPollIdAsync(int voterId, int pollId);
     
    }
}