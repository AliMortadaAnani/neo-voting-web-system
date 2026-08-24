using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GovernmentSystem.API.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Voter> Voters { get; set; }
        public DbSet<Candidate> Candidates { get; set; }

        public DbSet<Citizen> Citizens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Base Identity Configuration (Crucial for Users/Roles)
            base.OnModelCreating(modelBuilder);

            // 2. Voter Configuration
            modelBuilder.Entity<Voter>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Fields Configuration
                entity.Property(e => e.CitizenId).IsRequired();
                entity.HasIndex(e => e.CitizenId).IsUnique(); // Enforce Uniqueness

                entity.Property(e => e.VotingToken).IsRequired().HasMaxLength(1000);
                entity.HasIndex(e => e.VotingToken).IsUnique(); // Enforce Uniqueness

                entity.Property(e => e.HashedData).IsRequired().HasMaxLength(2000);
                entity.HasIndex(e => e.HashedData).IsUnique(); // Enforce Uniqueness
            });

            // 3. Candidate Configuration
            modelBuilder.Entity<Candidate>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Fields Configuration
                entity.Property(e => e.CitizenId).IsRequired();
                entity.HasIndex(e => e.CitizenId).IsUnique(); // Enforce Uniqueness

                entity.Property(e => e.NominationToken).IsRequired().HasMaxLength(1000);
                entity.HasIndex(e => e.NominationToken).IsUnique(); // Enforce Uniqueness

                entity.Property(e => e.HashedData).IsRequired().HasMaxLength(2000);
                entity.HasIndex(e => e.HashedData).IsUnique(); // Enforce Uniqueness
            });

            // 4. Citizen Configuration
            modelBuilder.Entity<Citizen>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Fields Configuration
                entity.Property(e => e.NationalId).IsRequired().HasMaxLength(1000);
                entity.HasIndex(e => e.NationalId).IsUnique(); // Enforce Uniqueness

                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DateOfBirth).IsRequired();
                entity.Property(e => e.Governorate).IsRequired();
                entity.Property(e => e.Gender).IsRequired().HasMaxLength(1);

                entity.ToTable(tb =>
                tb.HasCheckConstraint("CK_Citizen_Gender", "[Gender] IN ('M', 'F')"));

                // 1. Get all integer values from the Enum
                var enumValues = Enum.GetValues(typeof(GovernorateIdEnum))
                                     .Cast<int>();

                // 2. Create the SQL string: "1, 2, 3"
                var sqlValues = string.Join(", ", enumValues);

                // 3. Add the Check Constraint
                // SQL: CHECK ([GovernorateId] IN (1, 2, 3) OR [GovernorateId] IS NULL)
                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_Citizen_Governorate",
                    $"([Governorate] IN ({sqlValues}) )")
                );

                entity.HasOne<Voter>()
                      .WithOne(s => s.Citizen)
                      .HasForeignKey<Voter>(s => s.CitizenId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Candidate>()
                      .WithOne(s => s.Citizen)
                      .HasForeignKey<Candidate>(s => s.CitizenId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}