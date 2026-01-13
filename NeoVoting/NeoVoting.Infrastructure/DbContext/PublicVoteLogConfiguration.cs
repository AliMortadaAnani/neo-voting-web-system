using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoVoting.Domain.Entities;

namespace NeoVoting.Infrastructure.DbContext
{
    public class PublicVoteLogConfiguration : IEntityTypeConfiguration<PublicVoteLog>
    {
        public void Configure(EntityTypeBuilder<PublicVoteLog> builder)
        {
            // Primary key
            builder.HasKey(e => e.Id);
            builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
            // Properties
            builder.Property(e => e.TimestampUTC)
                .IsRequired();

            // Relationships
            builder.HasOne(e => e.Vote)
                .WithMany()
                .HasForeignKey(e => e.VoteId)//voteId is always present even if the vote is soft deleted
                .IsRequired(false)//because some votes might be soft deleted
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(pv => new { pv.VoteId }).IsUnique();
        }
    }
}