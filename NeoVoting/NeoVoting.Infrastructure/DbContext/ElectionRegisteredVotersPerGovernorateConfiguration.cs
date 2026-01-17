using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Infrastructure.DbContext
{
    public class ElectionRegisteredVotersPerGovernorateConfiguration : IEntityTypeConfiguration<ElectionRegisteredVotersPerGovernorate>
    {
        public void Configure(EntityTypeBuilder<ElectionRegisteredVotersPerGovernorate> builder)
        {
           

            // Primary key
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            // Foreign key: ElectionId
            builder.Property(e => e.ElectionId)
                .IsRequired();

            builder.HasOne(e => e.Election)
                .WithMany() // no back navigation
                .HasForeignKey(e => e.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Foreign key: GovernorateId
            builder.Property(e => e.GovernorateId)
                .IsRequired();

            builder.HasOne(e => e.Governorate)
                .WithMany() // no back navigation
                .HasForeignKey(e => e.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Count properties
            builder.Property(e => e.RegisteredVotersCount).IsRequired();
            builder.Property(e => e.RegisteredMalesCount).IsRequired();
            builder.Property(e => e.RegisteredFemalesCount).IsRequired();
            builder.Property(e => e.RegisteredAge18To29Count).IsRequired();
            builder.Property(e => e.RegisteredAge30To45Count).IsRequired();
            builder.Property(e => e.RegisteredAge46To64Count).IsRequired();
            builder.Property(e => e.RegisteredAge65AndOverCount).IsRequired();

            // Constraints
            builder.ToTable(tb =>
            {
                // Ensure no negative numbers
                tb.HasCheckConstraint("CK_Voters_NonNegative",
                    "[RegisteredVotersCount] >= 0 AND [RegisteredMalesCount] >= 0 AND [RegisteredFemalesCount] >= 0 " +
                    "AND [RegisteredAge18To29Count] >= 0 AND [RegisteredAge30To45Count] >= 0 AND [RegisteredAge46To64Count] >= 0 AND [RegisteredAge65AndOverCount] >= 0");

                
            });

            builder.HasIndex(cp => new { cp.ElectionId, cp.GovernorateId})
             .IsUnique();

            // 1. Get all integer values from the Enum
            var enumValues = Enum.GetValues(typeof(GovernoratesEnum))
                                 .Cast<int>();

            // 2. Create the SQL string: "1, 2, 3"
            var sqlValues = string.Join(", ", enumValues);

            builder.ToTable(t =>
               t.HasCheckConstraint("CK_ElectionStats_GovernorateId", $"([GovernorateId] IN ({sqlValues}))"));
        }
    }
}