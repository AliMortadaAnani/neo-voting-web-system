using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.Enums;

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
                .HasMaxLength(50);

            // Seed static lookup values
            builder.HasData(
                ElectionStatus.CreateUpcomingStatus(),
                ElectionStatus.CreateNominationStatus(),
                ElectionStatus.CreateVotingStatus(),
                ElectionStatus.CreateCompletedStatus()
            );
        }
    }

}
