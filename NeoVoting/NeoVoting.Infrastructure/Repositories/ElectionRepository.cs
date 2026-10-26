using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ElectionRepository : IElectionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ElectionRepository> _logger;

        public ElectionRepository(ApplicationDbContext context, ILogger<ElectionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsActiveElectionExistsAsync()
        {
            _logger.LogInformation("Checking whether any active election exists.");
            return await _context.Elections.AnyAsync(e => e.Status != StatusEnum.Completed);
        }

        public async Task<bool> IsElectionNameExistsAsync(string electionName)
        {
            _logger.LogInformation("Checking whether election name {ElectionName} already exists.", electionName);
            return await _context.Elections.AnyAsync(e => e.Name == electionName);
        }

        public void Add(Election election)
        {
            _logger.LogInformation("Adding election {ElectionId} named {ElectionName}.", election.Id, election.Name);
            _context.Elections.Add(election);
        }

        public async Task<Election?> GetByIdAsync(int electionId)
        {
            _logger.LogInformation("Fetching election {ElectionId}.", electionId);
            return await _context.Elections.FindAsync(electionId);
        }

        public async Task<List<Election>> GetPagedAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching paged elections page {PageNumber} size {PageSize}.", pageNumber, pageSize);
            return await _context.Elections
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            _logger.LogInformation("Counting elections.");
            return await _context.Elections.CountAsync();
        }

        public async Task<Election?> GetActiveElectionAsync()
        {
            _logger.LogInformation("Fetching active election.");
            return await _context.Elections.FirstOrDefaultAsync(e => e.Status != StatusEnum.Completed);
        }
    }
}