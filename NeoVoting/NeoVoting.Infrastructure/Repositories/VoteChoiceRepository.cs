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

        public async Task<VoteChoice?> GetVoteChoiceByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.VoteChoices
                .Include(vc => vc.Vote)
                .Include(vc => vc.CandidateProfile)
                .FirstOrDefaultAsync(vc => vc.Id == id, cancellationToken);
        }

        public async Task<List<VoteChoice>> GetAllVoteChoicesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.VoteChoices
                .Include(vc => vc.Vote)
                .Include(vc => vc.CandidateProfile)
                .ToListAsync(cancellationToken);
        }

        public async Task<VoteChoice> AddVoteChoiceAsync(VoteChoice voteChoice, CancellationToken cancellationToken)
        {
            await _dbContext.VoteChoices.AddAsync(voteChoice, cancellationToken);
            return voteChoice;
        }

       
    }
}