using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteChoiceRepository
    {
        Task<IReadOnlyList<VoteChoice>> GetAllVoteChoicesByCandidateProfileIdAsync(Guid CandidateProfileId, CancellationToken cancellationToken);

        Task<IReadOnlyList<VoteChoice>> GetPagedVoteChoicesByCandidateProfileIdAsync(Guid CandidateProfileId, int skip, int take, CancellationToken cancellationToken);

        Task<int> GetTotalVoteChoicesCountByCandidateProfileIdAsync(Guid CandidateProfileId, CancellationToken cancellationToken);

        Task<IReadOnlyList<VoteChoice>> GetVoteChoicesByVoteIdAsync(Guid VoteId, CancellationToken cancellationToken);

        Task<VoteChoice> AddVoteChoiceAsync(VoteChoice voteChoice, CancellationToken cancellationToken);

        void Delete(VoteChoice voteChoice);//in case Voted flag in Government System was not updated correctly to true we will soft delete the VoteChoice
    }
}