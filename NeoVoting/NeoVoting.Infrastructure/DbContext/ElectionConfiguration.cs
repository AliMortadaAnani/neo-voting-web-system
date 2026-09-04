using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Infrastructure.DbContext
{
    public class ElectionConfiguration : IEntityTypeConfiguration<Election>
    {
        public void Configure(EntityTypeBuilder<Election> builder)
        {
            // Primary key
            builder.HasKey(e => e.Id);
            builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
            // Name property
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                ; // adjust as needed
            builder.HasIndex(e => e.Name)
                .IsUnique();

            // Dates
            builder.Property(e => e.NominationStartDate)
                .IsRequired();

            builder.Property(e => e.NominationEndDate)
                .IsRequired();

            builder.Property(e => e.VotingStartDate)
                .IsRequired();

            builder.Property(e => e.VotingEndDate)
                .IsRequired();

            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<int>(); // Store enum as int in the database

            // 1. Get all integer values from the Enum
            var enumValues = Enum.GetValues(typeof(StatusEnum))
                                 .Cast<int>();

            // 2. Create the SQL string: "1, 2, 3"
            var sqlValues = string.Join(", ", enumValues);

            // 3. Add the Check Constraint
            // SQL: CHECK ([GovernorateId] IN (1, 2, 3) OR [GovernorateId] IS NULL)
            builder.ToTable(t =>
                t.HasCheckConstraint("CK_Election_Status",
                $"([Status] IN ({sqlValues}) )")
            );

            
        }
    }
}