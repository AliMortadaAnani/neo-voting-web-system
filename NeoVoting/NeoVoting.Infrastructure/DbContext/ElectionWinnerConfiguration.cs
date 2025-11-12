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

            // Relationships
            builder.HasOne(ew => ew.Election)
                .WithMany() // no back navigation from Election
                .HasForeignKey(ew => ew.ElectionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            

            builder.HasOne(ew => ew.CandidateProfile)
                .WithMany() // no back navigation from CandidateProfile
                .HasForeignKey(ew => ew.CandidateProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            // Prevent deleting a CandidateProfile if it is marked as a winner

            builder.HasIndex(ew => new { ew.ElectionId, ew.CandidateProfileId }).IsUnique();
        }
    }

}
