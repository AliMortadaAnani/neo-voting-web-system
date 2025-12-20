using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class VoteChoiceRepository : IVoteChoiceRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public VoteChoiceRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<VoteChoice> AddVoteChoiceAsync(VoteChoice voteChoice, CancellationToken cancellationToken)
        {
            await _dbContext.VoteChoices.AddAsync(voteChoice, cancellationToken);
            return voteChoice;
        }

        public void Delete(VoteChoice voteChoice)
        {
            _dbContext.VoteChoices.Remove(voteChoice);
        }

        public async Task<IReadOnlyList<VoteChoice>> GetAllVoteChoicesByCandidateProfileIdAsync(Guid CandidateProfileId, CancellationToken cancellationToken)
        {
            return await _dbContext.VoteChoices
                .Include(vc => vc.Vote)
                .Include(vc => vc.CandidateProfile)
                .Where(vc => vc.CandidateProfileId == CandidateProfileId)
                .OrderByDescending(vc => vc.Vote.TimestampUTC)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<VoteChoice>> GetPagedVoteChoicesByCandidateProfileIdAsync(Guid CandidateProfileId, int skip, int take, CancellationToken cancellationToken)
        {
            return await _dbContext.VoteChoices
                .Include(vc => vc.Vote)
                .Include(vc => vc.CandidateProfile)
                .Where(vc => vc.CandidateProfileId == CandidateProfileId)
                .OrderByDescending(vc => vc.Vote.TimestampUTC)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalVoteChoicesCountByCandidateProfileIdAsync(Guid CandidateProfileId, CancellationToken cancellationToken)
        {
            return await _dbContext.VoteChoices.CountAsync(vc => vc.CandidateProfileId == CandidateProfileId, cancellationToken);
        }

        public async Task<IReadOnlyList<VoteChoice>> GetVoteChoicesByVoteIdAsync(Guid VoteId, CancellationToken cancellationToken)
        {
            return await _dbContext.VoteChoices
                .Include(vc => vc.Vote)
                .Include(vc => vc.CandidateProfile)
                .ThenInclude(cp => cp.User)
                .Where(vc => vc.VoteId == VoteId)
                .OrderBy(vc => vc.CandidateProfile.User.UserName)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
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

        #endregion Soft Delete Demonstration
    }
}