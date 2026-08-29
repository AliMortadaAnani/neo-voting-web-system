using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IEventParticipationRepository
    {
        void Add(EventParticipation participation);

        Task<bool> HasVoterVotedByVoterIdAndElectionIdAsync(int voterId, int electionId);

        Task<bool> HasVoterVotedByVoterIdAndPollIdAsync(int voterId, int pollId);

        ///
        ///
        ///
        Task<int> GetCountOfTotalVotesByElectionIdAsync(int electionId);

        Task<int> GetCountOfVotesByElectionIdAndGenderAsync(int electionId, char gender);

        Task<int> GetCountOfVotesByElectionIdAndAgeRangeAsync(int electionId, int minAge, int maxAge);

        Task<int> GetCountOfVotesByElectionIdAndGovernorateAsync(int electionId, GovernorateIdEnum governorate);

        Task<int> GetCountOfVotesByElectionIdAndGenderAndGovernorateAsync(int electionId, char gender, GovernorateIdEnum governorate);

        Task<int> GetCountOfVotesByElectionIdAndAgePhaseAndGovernorateAsync(int electionId, int minAge, int maxAge, GovernorateIdEnum governorate);
    }
}