using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteChoiceRepository
    {
        Task<VoteChoice?> GetVoteChoiceByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<VoteChoice>> GetAllVoteChoicesAsync(CancellationToken cancellationToken);
        Task<VoteChoice> AddVoteChoiceAsync(VoteChoice voteChoice, CancellationToken cancellationToken);
        
    }
}
