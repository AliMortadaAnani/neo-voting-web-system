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
            // Properties
            builder.Property(v => v.VoterAge)
                .IsRequired();

            builder.Property(v => v.VoterGender)
                .IsRequired()
                .HasMaxLength(1);

            builder.Property(v => v.TimestampUTC)
                .IsRequired();

            // Relationships
            builder.HasOne(v => v.Election)
                .WithMany() // no back navigation
                .HasForeignKey(v => v.ElectionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Governorate)
                .WithMany() // no back navigation
                .HasForeignKey(v => v.GovernorateId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Table name and constraints
            builder.ToTable(tb =>
            {
                // GovernorateId must be between 1 and 5
                tb.HasCheckConstraint("CK_Vote_GovernorateId", "[GovernorateId] BETWEEN 1 AND 5");

                // VoterAge must be >= 18
                tb.HasCheckConstraint("CK_Vote_VoterAge", "[VoterAge] >= 18");

                // VoterGender must be 'M' or 'F'
                tb.HasCheckConstraint("CK_Vote_VoterGender", "[VoterGender] IN ('M','F')");
            });



        }
    }

}
