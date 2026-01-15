using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

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
                .HasMaxLength(1)
                .IsUnicode(false);

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
                // VoterAge must be >= 18
                tb.HasCheckConstraint("CK_Vote_VoterAge", "[VoterAge] >= 18");

                // VoterGender must be 'M' or 'F'
                tb.HasCheckConstraint("CK_Vote_VoterGender", "[VoterGender] IN ('M','F')");
            });


            // 1. Get all integer values from the Enum
            var enumValues = Enum.GetValues(typeof(GovernoratesEnum))
                                 .Cast<int>();

            // 2. Create the SQL string: "1, 2, 3"
            var sqlValues = string.Join(", ", enumValues);

            // 3. Add the Check Constraint
            // SQL: CHECK ([GovernorateId] IN (1, 2, 3) OR [GovernorateId] IS NULL)
            builder.ToTable(t =>
                t.HasCheckConstraint("CK_Voter_GovernorateId", $"([GovernorateId] IN ({sqlValues}) OR [GovernorateId] IS NULL)")
            );

            builder.HasIndex(v => v.IsDeleted);
        }
    }
}