using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Infrastructure.DbContext;

namespace GovernmentSystem.API.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;

        // The ApplicationDbContext is injected via the constructor.
        public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// The implementation simply delegates the call to the underlying DbContext's SaveChangesAsync method.
        /// This is where the actual database transaction happens.
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            _logger.LogInformation("UnitOfWork: Saving changes to database");
            int result = await _context.SaveChangesAsync();
            _logger.LogInformation("UnitOfWork: Changes saved successfully - Rows affected: {RowsAffected}", result);
            return result;
        }

        /// <summary>
        /// Disposes the underlying DbContext.
        /// </summary>
        public void Dispose()
        {
            //_logger.LogInformation("UnitOfWork: Disposing DbContext");
            _context.Dispose();
        }
    }
}