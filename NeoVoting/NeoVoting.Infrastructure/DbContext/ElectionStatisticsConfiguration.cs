using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Infrastructure.DbContext
{
    public class ElectionStatisticsConfiguration : IEntityTypeConfiguration<ElectionStatistics>
    {
        public void Configure(EntityTypeBuilder<ElectionStatistics> builder)
        {
            // Primary key
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.HasOne(e => e.Election)
                .WithMany(e => e.ElectionStatisticsList)
                .HasForeignKey(e => e.ElectionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}