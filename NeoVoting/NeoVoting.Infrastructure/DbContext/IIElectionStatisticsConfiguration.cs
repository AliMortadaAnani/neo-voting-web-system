using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Infrastructure.DbContext
{
    public class IIElectionStatisticsConfiguration : IEntityTypeConfiguration<ElectionStatistics>
    {
        public void Configure(EntityTypeBuilder<ElectionStatistics> builder)
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

            
            builder.HasOne(e => e.Governorate)
                .WithMany() // no back navigation
                .HasForeignKey(e => e.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            

            builder.HasIndex(cp => new { cp.ElectionId, cp.GovernorateId})
             .IsUnique();

            
        }
    }
}