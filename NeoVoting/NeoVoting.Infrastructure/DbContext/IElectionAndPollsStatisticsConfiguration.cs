using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

namespace NeoVoting.Infrastructure.DbContext
{
    public class IElectionAndPollsStatisticsConfiguration : IEntityTypeConfiguration<ElectionAndPollStatistics>
    {
        public void Configure(EntityTypeBuilder<ElectionAndPollStatistics> builder)
        {
           

            // Primary key
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();




            builder.HasOne(e => e.Election)
                .WithOne(e => e.ElectionAndPollStatistics) 
                .HasForeignKey<ElectionAndPollStatistics>(e => e.ElectionId)
                .IsRequired(false) // ElectionId is optional
                .OnDelete(DeleteBehavior.Restrict);

            
            builder.HasOne(e => e.Poll)
                .WithOne(e => e.ElectionAndPollStatistics)
                .HasForeignKey<ElectionAndPollStatistics>(e => e.PollId)
                .IsRequired(false) // PollId is optional
                .OnDelete(DeleteBehavior.Restrict);

            
            
        }
    }
}