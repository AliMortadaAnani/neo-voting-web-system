using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Infrastructure.DbContext
{
    public class CandidateProfileConfiguration : IEntityTypeConfiguration<CandidateProfile>
    {
        public void Configure(EntityTypeBuilder<CandidateProfile> builder)
        {
            // Primary key
            builder.HasKey(cp => cp.Id);
            builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
            // Properties

            builder.Property(cp => cp.NominationReasons)
                .IsRequired()
                .HasMaxLength(4000); // adjust as needed

            // Relationships
            builder.HasOne(cp => cp.Candidate)
                .WithMany(c => c.CandidateProfiles)
                .HasForeignKey(cp => cp.CandidateId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cp => cp.Election)
                .WithMany(e => e.CandidateProfiles)
                .HasForeignKey(cp => cp.ElectionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(cp => new { cp.ElectionId, cp.CandidateId })
              .IsUnique();
        }
    }
}