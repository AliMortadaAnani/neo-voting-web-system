using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Infrastructure.DbContext
{
    public class ElectionConfiguration : IEntityTypeConfiguration<Election>
    {
        public void Configure(EntityTypeBuilder<Election> builder)
        {
            // Primary key
            builder.HasKey(e => e.Id);
            builder.Property(entity => entity.Id).ValueGeneratedNever();
            // Name property
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                ; // adjust as needed
            builder.HasIndex(e => e.Name)
                .IsUnique();

            // Dates
            builder.Property(e => e.NominationStartDate)
                .IsRequired();

            builder.Property(e => e.NominationEndDate)
                .IsRequired();

            builder.Property(e => e.VotingStartDate)
                .IsRequired();

            builder.Property(e => e.VotingEndDate)
                .IsRequired();

            builder.Property(e => e.FinalNumberOfRegisteredVoters)
                .IsRequired(false); // nullable, set when election complete
            // Foreign key relationship
            builder.HasOne(e => e.ElectionStatus)
                .WithMany() // no back navigation
                .HasForeignKey(e => e.ElectionStatusId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Table name and constraints
            builder.ToTable(tb =>
            {
                // Example: enforce that NominationEndDate > NominationStartDate
                tb.HasCheckConstraint("CK_Election_NominationDates",
                    "[NominationEndDate] > [NominationStartDate]");

                // Example: enforce that VotingEndDate > VotingStartDate
                tb.HasCheckConstraint("CK_Election_VotingDates",
                    "[VotingEndDate] > [VotingStartDate]");

                // Example: enforce that VotingStartDate >= NominationEndDate
                tb.HasCheckConstraint("CK_Election_VotingAfterNomination",
                    "[VotingStartDate] >= [NominationEndDate]");

                tb.HasCheckConstraint("CK_Election_Name", "LEN([Name]) > 0");
            });
        }
    }
}