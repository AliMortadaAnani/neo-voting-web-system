using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IVoteRepository
    {
        Task<Vote?> GetVoteByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Vote>> GetAllVotesAsync(CancellationToken cancellationToken);
        Task<Vote> AddVoteAsync(Vote vote, CancellationToken cancellationToken);
        
    }
}
