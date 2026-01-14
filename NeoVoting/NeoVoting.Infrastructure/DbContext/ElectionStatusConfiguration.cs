using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Infrastructure.DbContext
{
    public class ElectionStatusConfiguration : IEntityTypeConfiguration<ElectionStatus>
    {
        public void Configure(EntityTypeBuilder<ElectionStatus> builder)
        {
            builder.HasKey(es => es.Id);
            builder.Property(entity => entity.Id).ValueGeneratedNever();
            builder.Property(es => es.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Seed static lookup values
            builder.HasData(
                ElectionStatus.CreateUpcomingStatus(),
                ElectionStatus.CreateNominationStatus(),
                ElectionStatus.CreatePreVotingStatus(),
                ElectionStatus.CreateVotingStatus(),
                ElectionStatus.CreateCompletedStatus()
            );
        }
    }
}