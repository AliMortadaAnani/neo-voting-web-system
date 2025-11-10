using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.IdentityEntities;


namespace NeoVoting.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<ElectionStatus> ElectionStatuses { get; set; }

        public virtual DbSet<Governorate> Governorates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This is crucial. It configures all the Identity tables (AspNetUsers, etc.) first.
            base.OnModelCreating(modelBuilder);

            // --- Governorate Configuration ---
            modelBuilder.Entity<Governorate>(entity =>
            {
                // Define the primary key
                entity.HasKey(g => g.Id);


                // Prevent the database from trying to auto-generate the ID for this table,
                // since we are providing the IDs manually from our factories/enum.
                entity.Property(es => es.Id).ValueGeneratedNever();

                // Validation: The Name is required and has a max length.
                // EF Core knows 'required string' means NOT NULL from your entity,
                // but explicitly defining length is a good practice.
                entity.Property(g => g.Name).IsRequired().HasMaxLength(100);

                // --- Seeding the Governorate Data ---
                // This data will be inserted into the database when the migration is applied.
                entity.HasData(
                    Governorate.CreateBeirutGovernorate(),
                    Governorate.CreateMountLebanonGovernorate(),
                    Governorate.CreateSouthGovernorate(),
                    Governorate.CreateEastGovernorate(),
                    Governorate.CreateNorthGovernorate()
                );

                // Note: By convention, EF Core will already configure the one-to-many 
                // relationship between Governorate and ApplicationUser.
            });

            // --- ElectionStatus Configuration ---
            modelBuilder.Entity<ElectionStatus>(entity =>
            {
                // Define the primary key
                entity.HasKey(es => es.Id);

                // Prevent the database from trying to auto-generate the ID for this table,
                // since we are providing the IDs manually from our factories/enum.
                entity.Property(es => es.Id).ValueGeneratedNever();

                // Validation: The Name is required and has a max length.
                entity.Property(es => es.Name).IsRequired().HasMaxLength(50);

                // --- Seeding the ElectionStatus Data ---
                entity.HasData(
                    ElectionStatus.CreateUpcomingStatus(),
                    ElectionStatus.CreateNominationStatus(),
                    ElectionStatus.CreateVotingStatus(),
                    ElectionStatus.CreateCompletedStatus()
                );
            });

            
        }
    }
}