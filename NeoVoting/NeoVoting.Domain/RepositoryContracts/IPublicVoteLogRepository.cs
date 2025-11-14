using NeoVoting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeoVoting.Domain.RepositoryContracts
{
    public interface IPublicVoteLogRepository
    {
        Task<PublicVoteLog?> GetPublicVoteLogByIdAsync(long logId, CancellationToken cancellationToken);
        Task<List<PublicVoteLog>> GetAllPublicVoteLogsAsync(CancellationToken cancellationToken);
        Task<PublicVoteLog> AddPublicVoteLogAsync(PublicVoteLog log, CancellationToken cancellationToken);
        
    }
}
