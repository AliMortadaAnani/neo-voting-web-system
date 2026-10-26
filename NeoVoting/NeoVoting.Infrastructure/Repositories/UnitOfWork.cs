using Microsoft.Extensions.Logging;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> SaveChangesAsync()
        {
            _logger.LogInformation("Saving database changes.");

            try
            {
                var saved = await _context.SaveChangesAsync();
                _logger.LogInformation("Database changes saved successfully. Rows affected: {RowsAffected}", saved);
                return saved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving database changes.");
                throw;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}