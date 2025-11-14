using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.IdentityEntities;
using System.Reflection;

namespace NeoVoting.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Add a DbSet for each of your custom entities. This tells EF Core
        // that you want to create a table for each of these.
        public DbSet<Election> Elections { get; set; }
        public DbSet<ElectionStatus> ElectionStatuses { get; set; }
        public DbSet<Governorate> Governorates { get; set; }
        public DbSet<CandidateProfile> CandidateProfiles { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<VoteChoice> VoteChoices { get; set; }
        public DbSet<ElectionWinner> ElectionWinners { get; set; }
        public DbSet<PublicVoteLog> PublicVoteLogs { get; set; }
        public DbSet<SystemAuditLog> SystemAuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This is essential for IdentityDbContext to work correctly.
            base.OnModelCreating(modelBuilder);

            // This is a powerful feature. It scans the current assembly (the Infrastructure project)
            // for all classes that implement IEntityTypeConfiguration and applies them automatically.
            // This keeps your DbContext class clean.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }

}
