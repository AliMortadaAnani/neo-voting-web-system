using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public VoteRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Vote> AddVoteAsync(Vote vote, CancellationToken cancellationToken)
        {
            await _dbContext.Votes.AddAsync(vote, cancellationToken);
            return vote;
        }

        public void Delete(Vote vote)
        {
            _dbContext.Votes.Remove(vote);
        }

        public async Task<IReadOnlyList<Vote>> GetAllVotesByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken)
        {
            return await _dbContext.Votes
                .Include(v => v.Election)
                .Include(v => v.Governorate)
                .Where(v => v.ElectionId == ElectionId)
                .OrderByDescending(v => v.TimestampUTC)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Vote>> GetPagedVotesByElectionIdAsync(Guid ElectionId, int skip, int take, CancellationToken cancellationToken)
        {
            return await _dbContext.Votes
                .Include(v => v.Election)
                .Include(v => v.Governorate)
                .Where(v => v.ElectionId == ElectionId)
                .OrderByDescending(v => v.TimestampUTC)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalVotesCountByElectionIdAsync(Guid ElectionId, CancellationToken cancellationToken)
        {
            return await _dbContext.Votes.CountAsync(v => v.ElectionId == ElectionId, cancellationToken);
        }

        public async Task<Vote?> GetVoteByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Votes
                .Include(v => v.Election)
                .Include(v => v.Governorate)
                .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        }
        #region Soft Delete Demonstration
        /*
         // SCENARIO 1: The Standard Delete
    // You call Remove, but the DbContext Interceptor converts it to Soft Delete
    public void Delete(Vote vote)
    {
        _dbContext.Votes.Remove(vote); 
    }

    // SCENARIO 2: Standard Get
    // Automatically EXCLUDES deleted items because of Global Query Filter
    public async Task<IReadOnlyList<Vote>> GetAllActiveVotesAsync(Guid electionId, CancellationToken ct)
    {
        return await _dbContext.Votes
            .Where(v => v.ElectionId == electionId)
            .AsNoTracking()
            .ToListAsync(ct);
        // SQL generated will include: "WHERE IsDeleted = 0 AND ElectionId = ..."
    }

    // SCENARIO 3: Admin/Audit Get
    // We want to see EVERYTHING (Active + Deleted)
    public async Task<IReadOnlyList<Vote>> GetAllVotesIncludingDeletedAsync(Guid electionId, CancellationToken ct)
    {
        return await _dbContext.Votes
            .IgnoreQueryFilters() // <--- This disables the automatic "IsDeleted = 0" check
            .Where(v => v.ElectionId == electionId)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    // SCENARIO 4: Get ONLY Deleted items (e.g., for a "Recycle Bin" view)
    public async Task<IReadOnlyList<Vote>> GetOnlyDeletedVotesAsync(Guid electionId, CancellationToken ct)
    {
        return await _dbContext.Votes
            .IgnoreQueryFilters() // First, disable the automatic filter
            .Where(v => v.ElectionId == electionId && v.IsDeleted == true) // Then explicitly ask for deleted
            .AsNoTracking()
            .ToListAsync(ct);
    }
         */
        #endregion
    }
}