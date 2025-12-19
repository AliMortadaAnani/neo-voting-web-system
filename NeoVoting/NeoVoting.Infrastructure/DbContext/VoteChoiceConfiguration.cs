using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Infrastructure.DbContext
{


    public class VoteChoiceConfiguration : IEntityTypeConfiguration<VoteChoice>
    {
        public void Configure(EntityTypeBuilder<VoteChoice> builder)
        {
            // Primary key
            builder.HasKey(vc => vc.Id);
            builder.Property(entity => entity.Id).ValueGeneratedNever();
            // Relationships
            builder.HasOne(vc => vc.Vote)
                .WithMany() // no back navigation from Vote
                .HasForeignKey(vc => vc.VoteId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(vc => vc.CandidateProfile)
                .WithMany() // no back navigation from CandidateProfile
                .HasForeignKey(vc => vc.CandidateProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            // Prevent deleting a CandidateProfile if votes reference it

            builder.HasIndex(vc => new { vc.VoteId, vc.CandidateProfileId })
              .IsUnique();


            builder.HasIndex(v => v.IsDeleted);

            // OR a filtered index (Better for performance)
            /*builder.HasIndex(vc => vc.ElectionId)
                   .HasFilter("[IsDeleted] = 0");*/



            /*Why ? 99 % of your queries will be WHERE ElectionId = X AND IsDeleted = 0.Without a filtered index, the DB scans deleted rows too.*/

        }
    }

}
