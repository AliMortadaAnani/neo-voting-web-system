using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;

namespace NeoVoting.Infrastructure.Repositories
{
    public class ApplicationUserRepository(ApplicationDbContext dbContext) : IApplicationUserRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        // Helper query to isolate "Voters"
        // This assumes your Identity Roles are seeded with a role named "Voter"
        private IQueryable<Domain.IdentityEntities.ApplicationUser> GetVotersQuery()
        {
            // Join Users -> UserRoles -> Roles to filter by Role Name "Voter"
            // Note: Adjust "Voter" string if your role name is different (e.g. "VoterUser")
            return from user in _dbContext.Users
                   join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                   join role in _dbContext.Roles on userRole.RoleId equals role.Id
                   where role.Name == "Voter"
                   select user;
        }

        public async Task<int> GetCountOfTotalVotersAsync(CancellationToken cancellationToken)
        {
            return await GetVotersQuery().CountAsync(cancellationToken);
        }

        public async Task<int> GetCountOfVotersByGovernorateIdAsync(int governorateId, CancellationToken cancellationToken)
        {
            return await GetVotersQuery()
                .Where(u => u.GovernorateId == governorateId)
                .CountAsync(cancellationToken);
        }

        public async Task<int> GetCountOfVotersByGenderAsync(char gender, CancellationToken cancellationToken)
        {
            // char.ToUpper to ensure case insensitivity
            char normalizedGender = char.ToUpper(gender);
            return await GetVotersQuery()
                .Where(u => u.Gender == normalizedGender) // Assuming 'Gender' exists on AppUser
                .CountAsync(cancellationToken);
        }

        public async Task<int> GetCountOfVotersByAgeRangeAsync(int minAge, int maxAge, CancellationToken cancellationToken)
        {
            
            var today = DateTime.UtcNow.Date;
            var maxDateOfBirth = today.AddYears(-minAge);      // e.g. Born <= 2007 (for 18)
            var minDateOfBirth = today.AddYears(-(maxAge + 1)); // e.g. Born > 1925 (for 100)

            return await GetVotersQuery()
                .Where(u => u.DateOfBirth <= maxDateOfBirth && u.DateOfBirth > minDateOfBirth)
                .CountAsync(cancellationToken);
        }

        public async Task<int> GetCountOfVotersByGovernorateAndGenderAsync(int governorateId, char gender, CancellationToken cancellationToken)
        {
            char normalizedGender = char.ToUpper(gender);
            return await GetVotersQuery()
               .Where(u => u.GovernorateId == governorateId && u.Gender == normalizedGender)
               .CountAsync(cancellationToken);
        }

        public async Task<int> GetCountOfVotersByGovernorateAndAgeRangeAsync(int governorateId, int minAge, int maxAge, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var maxDateOfBirth = today.AddYears(-minAge);
            var minDateOfBirth = today.AddYears(-(maxAge + 1));

            return await GetVotersQuery()
                .Where(u => u.GovernorateId == governorateId &&
                            u.DateOfBirth <= maxDateOfBirth &&
                            u.DateOfBirth > minDateOfBirth)
                .CountAsync(cancellationToken);
        }

        

       
    }
}
