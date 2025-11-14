using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class SystemAuditLogRepository : ISystemAuditLog
    {
        private readonly ApplicationDbContext _dbContext;
        public SystemAuditLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // Implementation will be added later
    }
}