using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Infrastructure.DbContext
{
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            // Primary key
            builder.HasKey(cp => cp.Id);
            builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
            // Properties
            builder.Property(cp => cp.FirstName)
                .IsRequired()
                .HasMaxLength(100); // adjust as needed

            builder.Property(cp => cp.LastName)
                .IsRequired()
                .HasMaxLength(100); // adjust as needed

            builder.Property(cp => cp.DateOfBirth)
                .IsRequired();

            builder.Property(v => v.Governorate)
                .IsRequired()
                .HasConversion<int>(); // Store enum as int in the database

            // 1. Get all integer values from the Enum
            var enumValues = Enum.GetValues(typeof(GovernorateIdEnum))
                                 .Cast<int>();

            // 2. Create the SQL string: "1, 2, 3"
            var sqlValues = string.Join(", ", enumValues);

            // 3. Add the Check Constraint
            // SQL: CHECK ([GovernorateId] IN (1, 2, 3) )
            builder.ToTable(t =>
                t.HasCheckConstraint("CK_Candidate_Governorate", $"([Governorate] IN ({sqlValues}) )")
            );

            builder.Property(v => v.Gender)
              .IsRequired()
              .HasMaxLength(1);

            builder.ToTable(tb =>
               tb.HasCheckConstraint("CK_Candidate_Gender", "[Gender] IN ('M', 'F')"));

            builder.Property(cp => cp.VerificationHash)
                .IsRequired()
                .HasMaxLength(2000); // adjust as needed

            builder.HasOne(ew => ew.User)
                .WithOne(cp => cp.Candidate)
                .HasForeignKey<Candidate>(ew => ew.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(ew => new { ew.UserId }).IsUnique();
        }
    }
}