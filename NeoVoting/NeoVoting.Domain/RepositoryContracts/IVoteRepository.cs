using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteRepository
    {
        Task<Vote> AddVoteAsync(Vote vote, CancellationToken cancellationToken);

        void Delete(Vote vote);//in case Voted flag in Government System was not updated correctly to true we will soft delete the Vote

        Task<int> GetCountOfTotalVotesByElectionIdAsync(Guid electionId, CancellationToken cancellationToken);

        Task<int> GetCountOfVotesByElectionIdAndGenderAsync(Guid electionId, char gender, CancellationToken cancellationToken);

        Task<int> GetCountOfVotesByElectionIdAndAgeRangeAsync(Guid electionId, int minAge, int maxAge, CancellationToken cancellationToken);

        Task<int> GetCountOfVotesByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, CancellationToken cancellationToken);

        Task<int> GetCountOfVotesByElectionIdAndGenderAndGovernorateIdAsync(Guid electionId, char gender, int governorateId, CancellationToken cancellationToken);

        Task<int> GetCountOfVotesByElectionIdAndAgePhaseAndGovernorateIdAsync(Guid electionId, int minAge, int maxAge, int governorateId, CancellationToken cancellationToken);
    }
}