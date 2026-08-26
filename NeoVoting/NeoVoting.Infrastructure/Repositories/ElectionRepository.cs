using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionRepository : IElectionRepository
    {
        private readonly ApplicationDbContext _context;

        public ElectionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Election election)
        {
            _context.Elections.Add(election);
        }

        public async Task<bool> IsActiveElectionExistsAsync()
        {
            return await _context.Elections.AnyAsync(e => e.Status == StatusEnum.Voting);
        }

        public async Task<List<Election>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Elections
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Elections.CountAsync();
        }

        public async Task<Election?> GetByIdAsync(int electionId)
        {
            return await _context.Elections.FindAsync(electionId);
        }

        public async Task<Election?> GetByNameAsync(string electionName)
        {
            return await _context.Elections.FirstOrDefaultAsync(e => e.Name == electionName);
        }

        public async Task<Election?> GetActiveElectionAsync()
        {
            return await _context.Elections.FirstOrDefaultAsync(e => e.Status == StatusEnum.Voting);
        }
    }
}