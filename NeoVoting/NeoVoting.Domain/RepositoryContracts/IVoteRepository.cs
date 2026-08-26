using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteRepository
    {
        void Add(Vote vote);

    
        Task<bool> IsVoteChoicesForVoteEqualFive(Vote vote); 

        Task<Vote?> GetByVoteId(Guid voteId);

        Task<List<Vote>> GetPagedByElectionIdAsync(int electionId, int pageNumber, int pageSize);

        Task<int> GetCountOfTotalVotesByElectionIdAsync(int electionId);

        Task<int> GetCountOfVotesByElectionIdAndGenderAsync(int electionId, char gender);

        Task<int> GetCountOfVotesByElectionIdAndAgeRangeAsync(int electionId, int minAge, int maxAge);

        Task<int> GetCountOfVotesByElectionIdAndGovernorateAsync(int electionId, GovernorateIdEnum governorate);

        Task<int> GetCountOfVotesByElectionIdAndGenderAndGovernorateAsync(int electionId, char gender, GovernorateIdEnum governorate);

        Task<int> GetCountOfVotesByElectionIdAndAgePhaseAndGovernorateAsync(int electionId, int minAge, int maxAge, GovernorateIdEnum governorate);


    }
}