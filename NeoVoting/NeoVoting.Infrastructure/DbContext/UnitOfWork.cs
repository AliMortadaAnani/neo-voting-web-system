using NeoVoting.Domain.Contracts;

namespace NeoVoting.Infrastructure.DbContext
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        // The ApplicationDbContext is injected via the constructor.
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// The implementation simply delegates the call to the underlying DbContext's SaveChangesAsync method.
        /// This is where the actual database transaction happens.
        /// </summary>
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Disposes the underlying DbContext.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}