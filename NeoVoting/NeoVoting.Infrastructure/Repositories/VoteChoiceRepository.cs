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
            _dbContext.VoteChoices.Remove(voteChoice); // soft deleted
        }


        public async Task<IReadOnlyList<Guid>> GetTop5CandidatesIdsPerGovernorate(
    Guid electionId,
    int governorateId,
    CancellationToken cancellationToken)
        {
            var query = _dbContext.VoteChoices
                //.Include(vc => vc.Vote)
                //.ThenInclude(v => v.Election)
                //.Include(vc => vc.Vote)
                //.ThenInclude(v => v.Governorate)

                .Where(vc =>
                vc.Vote.ElectionId == electionId
                &&
                vc.Vote.GovernorateId == governorateId)
                .GroupBy(vc => vc.CandidateProfileId)
                .OrderByDescending(g => g.Count())
                .ThenBy(u => Guid.NewGuid())
                .Take(5)
                .Select(g => g.Key)
                ;
            return await query.ToListAsync(cancellationToken);
        }
   

        public async Task<int> GetCountOfTotalVoteChoicesByCandidateProfileIdAsync(Guid CandidateProfileId, CancellationToken cancellationToken)
        {
            return await _dbContext.VoteChoices.CountAsync(vc => vc.CandidateProfileId == CandidateProfileId, cancellationToken);
        }


        public async Task<bool> IsVoteChoiceExistsByVoteIdAndCandidateProfileIdAsync(Guid voteId, Guid candidateProfileId, CancellationToken cancellationToken)
        {
            return await _dbContext.VoteChoices
                .AnyAsync(vc => vc.VoteId == voteId && vc.CandidateProfileId == candidateProfileId, cancellationToken);
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