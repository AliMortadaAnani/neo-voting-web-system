using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Infrastructure.DbContext
{
    public class ElectionWinnerConfiguration : IEntityTypeConfiguration<ElectionWinner>
    {
        public void Configure(EntityTypeBuilder<ElectionWinner> builder)
        {
            // Primary key
            builder.HasKey(ew => ew.Id);
            builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
            // Properties
            builder.Property(ew => ew.VoteCount)
                .IsRequired(false); // nullable, allows recounts or non-applicable



            // RELATIONSHIP: ElectionWinner HAS ONE CandidateProfile
            builder.HasOne(ew => ew.CandidateProfile)       // Navigation property on ElectionWinner
                .WithOne(cp => cp.ElectionWinner)           // Navigation property on CandidateProfile (if you add it)
                .HasForeignKey<ElectionWinner>(ew => ew.CandidateProfileId) // Foreign key is on ElectionWinner
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(ew => new { ew.CandidateProfileId }).IsUnique();
        }
    }
}