using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteRepository
    {
        Task<Vote?> GetVoteByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<Vote>> GetAllVotesByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken);

        Task<IReadOnlyList<Vote>> GetPagedVotesByElectionIdAsync(Guid ElectionId, int skip, int take, CancellationToken cancellationToken);

        Task<int> GetTotalVotesCountByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken);

        Task<Vote> AddVoteAsync(Vote vote, CancellationToken cancellationToken);

        void Delete(Vote vote);//in case Voted flag in Government System was not updated correctly to true we will soft delete the Vote

        Task<int> GetVotesCountByElectionIdAndGenderAsync(Guid electionId, char gender, CancellationToken cancellationToken);

        Task<int> GetVotesCountByElectionIdAndAgePhaseAsync(Guid electionId, int minAge, int maxAge, CancellationToken cancellationToken);

        Task<int> GetVotesCountByElectionIdAndGovernorateIdAsync(Guid electionId, int governorateId, CancellationToken cancellationToken);

        Task<int> GetVotesCountByElectionGenderAndGovernorateAsync(Guid electionId, char gender, int governorateId, CancellationToken cancellationToken);

        Task<int> GetVotesCountByElectionAgeAndGovernorateAsync(Guid electionId, int minAge, int maxAge, int governorateId, CancellationToken cancellationToken);
    }
}