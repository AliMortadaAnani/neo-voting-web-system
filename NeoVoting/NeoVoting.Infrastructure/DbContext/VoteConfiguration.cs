using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Infrastructure.DbContext
{
    public class VoteConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            // Primary key
            builder.HasKey(v => v.Id);
            builder.Property(entity => entity.Id).ValueGeneratedNever();

            builder.Property(v => v.TimestampUTC)
                .IsRequired();

            // Relationships
            builder.HasOne(v => v.Election)
                .WithMany(e => e.Votes) // no back navigation
                .HasForeignKey(v => v.ElectionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(vc => vc.CandidateProfile)
                .WithMany(cp => cp.Votes)
                .HasForeignKey(vc => vc.CandidateProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            // Prevent deleting a CandidateProfile if votes reference it
        }
    }
}