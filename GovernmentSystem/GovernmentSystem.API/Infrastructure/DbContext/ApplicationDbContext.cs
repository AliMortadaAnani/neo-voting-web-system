using GovernmentSystem.API.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GovernmentSystem.API.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Voter> Voters { get; set; }
        public DbSet<Candidate> Candidates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Base Identity Configuration (Crucial for Users/Roles)
            base.OnModelCreating(modelBuilder);

            // 2. Voter Configuration
            modelBuilder.Entity<Voter>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);
                // We generate GUIDs in the C# "Create" method, so tell EF not to generate them on add
                entity.Property(e => e.Id).ValueGeneratedNever();

                // Fields Configuration
                entity.Property(e => e.NationalId).IsRequired();
                entity.HasIndex(e => e.NationalId).IsUnique(); // Enforce Uniqueness

                entity.Property(e => e.VotingToken).IsRequired();
                entity.HasIndex(e => e.VotingToken).IsUnique(); // Enforce Uniqueness

                entity.Property(e => e.GovernorateId)
                      .IsRequired()
                      .HasConversion<int>(); // Ensure Enum is stored as Int

                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DateOfBirth).IsRequired();

                entity.Property(e => e.Gender)
                      .IsRequired()
                      .HasMaxLength(1) // Maps to CHAR(1) or NVARCHAR(1)
                      .IsUnicode(false); // Use CHAR(1) for better performance

                entity.Property(e => e.EligibleForElection).IsRequired();
                entity.Property(e => e.ValidToken).IsRequired();
                entity.Property(e => e.IsRegistered).IsRequired();
                entity.Property(e => e.Voted).IsRequired();

                // SQL Check Constraints (The Guards)
                // Ensure Gender is only M or F
                entity.ToTable(t => t.HasCheckConstraint("CK_Voters_Gender", "[Gender] IN ('M', 'F')"));
                // Ensure GovernorateId is between 1 and 5
                entity.ToTable(t => t.HasCheckConstraint("CK_Voters_GovernorateId", "[GovernorateId] BETWEEN 1 AND 5"));
            });

            // 3. Candidate Configuration
            modelBuilder.Entity<Candidate>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                // Fields Configuration
                entity.Property(e => e.NationalId).IsRequired();
                entity.HasIndex(e => e.NationalId).IsUnique(); // Enforce Uniqueness

                entity.Property(e => e.NominationToken).IsRequired();
                entity.HasIndex(e => e.NationalId).IsUnique(); // Enforce Uniqueness

                entity.Property(e => e.GovernorateId)
                      .IsRequired()
                      .HasConversion<int>();

                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DateOfBirth).IsRequired();

                entity.Property(e => e.Gender)
                      .IsRequired()
                      .HasMaxLength(1)
                      .IsUnicode(false);

                entity.Property(e => e.EligibleForElection).IsRequired();
                entity.Property(e => e.ValidToken).IsRequired();
                entity.Property(e => e.IsRegistered).IsRequired();

                // SQL Check Constraints
                entity.ToTable(t => t.HasCheckConstraint("CK_Candidates_Gender", "[Gender] IN ('M', 'F')"));
                entity.ToTable(t => t.HasCheckConstraint("CK_Candidates_GovernorateId", "[GovernorateId] BETWEEN 1 AND 5"));
            });
        }
    }
}