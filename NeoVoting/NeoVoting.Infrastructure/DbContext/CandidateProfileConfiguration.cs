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
            builder.Property(entity => entity.Id).ValueGeneratedNever();
            // Properties
            builder.Property(cp => cp.Goals)
                .IsRequired()
                .HasMaxLength(1000); // adjust as needed

            builder.Property(cp => cp.NominationReasons)
                .IsRequired()
                .HasMaxLength(1000); // adjust as needed

            builder.Property(cp => cp.ProfilePhotoUrl)
                .HasMaxLength(500); // optional, allow null

            // Relationships
            builder.HasOne(cp => cp.User)
                .WithMany() // no back navigation from ApplicationUser
                .HasForeignKey(cp => cp.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cp => cp.Election)
                .WithMany() // no back navigation from Election
                .HasForeignKey(cp => cp.ElectionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            //NO!
            // Cascade makes sense: if an Election is deleted, its CandidateProfiles should go too

            // Table name and constraints
            builder.ToTable(tb =>
            {
                // Example: ensure Goals and NominationReasons are not empty strings
                tb.HasCheckConstraint("CK_CandidateProfile_Goals", "LEN([Goals]) > 0");
                tb.HasCheckConstraint("CK_CandidateProfile_NominationReasons", "LEN([NominationReasons]) > 0");
            });


            builder.HasIndex(cp => new { cp.UserId, cp.ElectionId })
              .IsUnique();

        }
    }

}
