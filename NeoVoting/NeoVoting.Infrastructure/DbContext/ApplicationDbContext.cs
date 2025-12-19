using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Contracts;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.IdentityEntities;
using System.Linq.Expressions;
using System.Reflection;

namespace NeoVoting.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Domain Tables
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
            // Essential for Identity framework keys and tables
            base.OnModelCreating(modelBuilder);

            // ---------------------------------------------------------
            // 1. GLOBAL QUERY FILTER (Soft Delete)
            // ---------------------------------------------------------
            // Automatically appends "WHERE IsDeleted = false" to all queries 
            // for entities that implement ISoftDeletable.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                                .HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
                }
            }

            // ---------------------------------------------------------
            // 2. FLUENT API CONFIGURATIONS
            // ---------------------------------------------------------
            // Scans the Infrastructure project for separate configuration files (IEntityTypeConfiguration)
            // ensuring this OnModelCreating method stays clean.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // ---------------------------------------------------------
            // SOFT DELETE INTERCEPTOR
            // ---------------------------------------------------------
            // Detects deletions before they hit the DB and converts them to updates.
            foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
            {
                switch (entry.State)
                {
                    // Scenario 1: Service called _dbContext.Remove(entity)
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;       // Cancel physical delete
                        entry.Entity.IsDeleted = true;            // Set Soft Delete flag
                        entry.Entity.DeletedAt = DateTimeOffset.UtcNow; // Audit time
                        break;

                    // Scenario 2: Service manually set entity.IsDeleted = true/false
                    case EntityState.Modified:
                        if (entry.Property(x => x.IsDeleted).IsModified)
                        {
                            // If marking as Deleted -> Set Timestamp
                            // If restoring (Un-deleting) -> Clear Timestamp
                            entry.Entity.DeletedAt = entry.Entity.IsDeleted
                                ? DateTimeOffset.UtcNow
                                : null;
                        }
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        // ---------------------------------------------------------
        // HELPERS
        // ---------------------------------------------------------

        // dynamically builds the LINQ expression: x => !x.IsDeleted
        private static LambdaExpression ConvertFilterExpression(Type entityType)
        {
            // 1. The Parameter "e"
            var newParam = Expression.Parameter(entityType);

            // 2. The Property "e.IsDeleted"
            var property = Expression.Property(newParam, nameof(ISoftDeletable.IsDeleted));

            // 3. The Logic "!e.IsDeleted"
            var notDeleted = Expression.Not(property);

            // 4. The Lambda "e => !e.IsDeleted"
            return Expression.Lambda(notDeleted, newParam);
        }
    }
}