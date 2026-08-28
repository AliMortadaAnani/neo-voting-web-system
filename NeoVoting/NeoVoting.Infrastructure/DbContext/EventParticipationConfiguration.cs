using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Infrastructure.DbContext
{
    public class EventParticipationConfiguration : IEntityTypeConfiguration<EventParticipation>
    {
        public void Configure(EntityTypeBuilder<EventParticipation> builder)
        {
            // Primary key
            builder.HasKey(v => v.Id);
            builder.Property(entity => entity.Id).ValueGeneratedOnAdd();

            builder.HasOne(ew => ew.Poll)
               .WithMany(cp => cp.EventParticipations)
               .HasForeignKey(ew => ew.PollId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ew => ew.Election)
               .WithMany(cp => cp.EventParticipations)
               .HasForeignKey(ew => ew.ElectionId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(v => new { v.VoterId, v.ElectionId }).IsUnique().HasFilter("[ElectionId] IS NOT NULL");

            builder.HasIndex(v => new { v.VoterId, v.PollId }).IsUnique().HasFilter("[PollId] IS NOT NULL");
        }
    }
}