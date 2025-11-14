using NeoVoting.Domain.Entities;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteChoiceRepository
    {
        Task<VoteChoice?> GetVoteChoiceByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<VoteChoice>> GetAllVoteChoicesAsync(CancellationToken cancellationToken);
        Task<VoteChoice> AddVoteChoiceAsync(VoteChoice voteChoice, CancellationToken cancellationToken);

    }
}
