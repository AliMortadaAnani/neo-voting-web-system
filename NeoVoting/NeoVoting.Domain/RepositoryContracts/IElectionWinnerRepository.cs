using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IElectionWinnerRepository
    {
        Task<ElectionWinner?> GetWinnerByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<ElectionWinner>> GetAllWinnersAsync(CancellationToken cancellationToken);
        Task<ElectionWinner> AddWinnerAsync(ElectionWinner winner, CancellationToken cancellationToken);
        void Update(ElectionWinner winner);
    }
}
